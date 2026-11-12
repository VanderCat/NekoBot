using System.Buffers.Binary;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace NekoBot.Adapter.Telegram.MTProto;

public class SocksTransport : ITransport {
    private AddressType _addressType;
    private readonly ITransport _innerTransport;
    private string _domainName = "";
    private IPAddress _ip = new(0);
    private ushort _port;
    public SocksTransport(ITransport innerTransport, IPAddress destinationAddress, int port) {
        _innerTransport = innerTransport;
        _ip = destinationAddress;
        _port = (ushort)port;
        switch (destinationAddress.AddressFamily) {
            case AddressFamily.InterNetwork:
                _addressType = AddressType.Ipv4;
                break;
            case AddressFamily.InterNetworkV6:
                _addressType = AddressType.Ipv6;
                break;
            default:
                throw new ArgumentException("only IP and IPV6 address are supported.", nameof(destinationAddress));
        }
    }

    public SocksTransport(ITransport innerTransport, string destinationDomain, int port) {
        _innerTransport = innerTransport;
        _domainName = destinationDomain;
        _port = (ushort)port;
    }

    private enum Command : byte {
        Connect = 1,
        Bind = 2,
        UdpAssociate = 3,
    }

    private enum AddressType : byte {
        Ipv4 = 1,
        DomainName = 3,
        Ipv6 = 4,
    }

    private enum Status : byte {
        Succeeded = 0,
        GeneralSocksServerFailure = 1,
        ConnectionNotAllowedByRuleset = 2,
        NetworkUnreachable = 3,
        HostUnreachable = 4,
        ConnectionRefused = 5,
        TtlExpired = 6,
        CommandNotSupported = 7,
        AddressTypeNotSupported = 8
    }

    private enum Authentication : byte {
        NoAuthentication = 0x0,
        GssApi = 0x1,
        UserPass = 0x2,
        NoAcceptableMethods = 0xFF
    }
    [StructLayout(LayoutKind.Sequential)]
    private struct HandshakeMessage {
        public byte Version;
        public byte MethodsCount;
        public Authentication AuthenticationMethod;
    }
    [StructLayout(LayoutKind.Sequential)]
    private struct HandshakeReply {
        public byte Version;
        public Authentication SelectedMethod;
    }
    [StructLayout(LayoutKind.Sequential)]
    private struct RequestHeader {
        public byte Version;
        public Command Command;
        private byte _reserved;
        public AddressType AddressType;
    }
    
    [StructLayout(LayoutKind.Sequential)]
    private struct RequestReplyHeader {
        public byte Version;
        public Status Status;
        private byte _reserved;
        public AddressType AddressType;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Span<byte> _<T>(ref T array) where T: unmanaged =>
        MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref array, 1));

    public void Connect() {
        Handshake();
        SendCommand(Command.Connect);
        ReadReply();
    }

    // yeah, i know this is a lot of unsafe but if you want to make it safe i wont stop you, please do so
    // add no excessive allocations though.
    private unsafe void SendCommand(Command command) {
        var variableSize = _addressType switch {
            AddressType.Ipv4 => 4,
            AddressType.Ipv6 => 16,
            AddressType.DomainName => _domainName.Length + 1,   // this is obviously incorrect, but since doamins and in
                                                                // worst case scenario puny code encoded domain still econded
                                                                // as ascii, it does not matter
            _ => throw new ArgumentOutOfRangeException()
        };
        var packet = stackalloc byte[sizeof(RequestHeader) + 2 + variableSize];
        *(RequestHeader*)packet = new RequestHeader {
            Version = 5,
            AddressType = _addressType,
            Command = command
        };
        int written;
        switch (_addressType) {
            case AddressType.Ipv6:
            case AddressType.Ipv4:
                if (!_ip.TryWriteBytes(new Span<byte>(packet + sizeof(RequestHeader), variableSize),
                    out written)) throw new IOException("Failed to write an IP address");
                break;
            case AddressType.DomainName:
                *(packet + sizeof(RequestHeader)) = (byte)_domainName.Length; //TODO: add check if string is less than 255 bytes
                Encoding.ASCII.GetBytes(_domainName,
                    new Span<byte>(packet + sizeof(RequestHeader) + 1, _domainName.Length));
                written = 1 + _domainName.Length;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        BinaryPrimitives.WriteUInt16BigEndian(new Span<byte>(packet + sizeof(RequestHeader) + written, 2), _port);
        _innerTransport.Send(new ReadOnlySpan<byte>(packet, sizeof(RequestHeader)+2+variableSize));
    }

    private void ReadReply() {
        var reply = new RequestReplyHeader();
        _innerTransport.ReceiveExact(_(ref reply));
        switch (reply.Status) {
            case Status.Succeeded:
                break;
            case Status.GeneralSocksServerFailure:
                throw new IOException("General SOCKS server failure.");
            case Status.ConnectionNotAllowedByRuleset:
                throw new IOException("Connection not allowed by ruleset.");
            case Status.NetworkUnreachable:
                throw new IOException("Network Unreachable.");
            case Status.HostUnreachable:
                throw new IOException("Host Unreachable.");
            case Status.ConnectionRefused:
                throw new IOException("Connection Refused.");
            case Status.TtlExpired:
                throw new IOException("TTL Expired.");
            case Status.CommandNotSupported:
                throw new NotSupportedException("Command not supported.");
            case Status.AddressTypeNotSupported:
                throw new NotSupportedException("Address Type not supported.");
            default:
                throw new ArgumentOutOfRangeException();
        }

        switch (reply.AddressType) {
            case AddressType.Ipv6:
            case AddressType.Ipv4:
                Span<byte> addr = stackalloc byte[2 + reply.AddressType switch {
                    AddressType.Ipv4 => 4,
                    AddressType.Ipv6 => 16
                }];
                _innerTransport.ReceiveExact(addr);
                break;
            case AddressType.DomainName:
                byte len = 0;
                _innerTransport.ReceiveExact(_(ref len));
                Span<byte> str = stackalloc byte[len];
                _innerTransport.ReceiveExact(str);
                var managed_str = Encoding.ASCII.GetString(str);
                _innerTransport.ReceiveExact(stackalloc byte[2]);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    private void Handshake() {
        var greeting = new HandshakeMessage {
            Version = 5,
            AuthenticationMethod = Authentication.NoAuthentication,
            MethodsCount = 1,
        };
        _innerTransport.Send(_(ref greeting));
        var resp = new HandshakeReply();
        _innerTransport.ReceiveExact(_(ref resp));
        if (resp.Version != 0x05)
            throw new IOException("Not a SOCKS5 proxy.");
        if (resp.SelectedMethod == Authentication.NoAcceptableMethods)
            throw new NotSupportedException("SOCKS5 authentication is not yet supported.");
    }

    public void Send(ReadOnlySpan<byte> packet) => _innerTransport.Send(packet);
    public ReadOnlyMemory<byte> Receive() => _innerTransport.Receive();

    public int ReceiveExact(Span<byte> packet) => _innerTransport.ReceiveExact(packet);
}