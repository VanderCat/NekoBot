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

namespace NekoBot.Telegram.MTProto;

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