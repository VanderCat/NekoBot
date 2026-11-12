using System.Net;
using System.Net.Sockets;

namespace NekoBot.Adapter.Telegram.MTProto;

public class SocketTransport(Socket socket, IPAddress ip, int port) : ITransport {
    public void Connect() {
        socket.Connect(ip, port);
    }
    public void Send(ReadOnlySpan<byte> packet) {
        var total = 0;
        while (total < packet.Length) {
            var sent = socket.Send(packet[total..]);
            if (sent <= 0)
                throw new SocketException((int)SocketError.ConnectionReset);
            total += sent;
        }
    }
    public ReadOnlyMemory<byte> Receive() {
        throw new NotSupportedException();
    }

    public int ReceiveExact(Span<byte> packet) {
        var total = 0;
        while (total < packet.Length) {
            var read = socket.Receive(packet[total..]);
            if (read == 0)
                throw new EndOfStreamException("Remote socket closed the connection unexpectedly.");
            total += read;
        }
        return total;

    }
}