using System.Net;
using System.Net.Sockets;
using NekoBot.Adapter.Telegram.MTProto;

namespace NekoBot.Adapter.Telegram;

public class TelegramApi : IChatApi {
    public string Name => "Telegram";
    public void Init() {
        var socket = new SocketTransport(new Socket(SocketType.Stream, ProtocolType.Tcp), IPAddress.Loopback, 2080);
        Console.WriteLine("connecting to localhost:2080");
        socket.Connect();
        var socks = new SocksTransport(socket, IPAddress.Parse("149.154.167.51"), 443);
        Console.WriteLine("connecting to 149.154.167.50:443");
        socks.Connect();
        var transport = new AbridgedTransport(socks);
        transport.Connect();
        Console.WriteLine("setting up abridged connection");
        var session = new MTProtoSession(transport);
        Console.WriteLine("initing the session");
        session.Connect();
    }
}