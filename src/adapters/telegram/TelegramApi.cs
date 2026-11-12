// Copyright 2026 VanderCat

// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at

//     http://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Net;
using System.Net.Sockets;
using NekoBot.Telegram.MTProto;

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