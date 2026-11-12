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

namespace NekoBot.Telegram.MTProto;

public interface ITransport {
    /// <summary>
    /// Connect to the server using handshake.
    /// </summary>
    /// <returns></returns>
    void Connect();
    /// <summary>
    /// Send packet.
    /// </summary>
    /// <param name="packet"></param>
    /// <returns></returns>
    void Send(ReadOnlySpan<byte> packet);
    /// <summary>
    /// Receive response.
    /// </summary>
    /// <returns></returns>
    ReadOnlyMemory<byte> Receive();

    /// <summary>
    /// Receive response.
    /// </summary>
    /// <param name="packet">a span with a defined length to read</param>
    /// <returns>Ammount of bytes read</returns>
    int ReceiveExact(Span<byte> packet);
    
    int ReceiveExact(byte[] packet, int offset, int count) => ReceiveExact(packet.AsSpan(offset..(offset + count)));
}