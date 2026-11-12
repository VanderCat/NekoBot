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

using System.Buffers.Binary;

namespace NekoBot.Telegram.MTProto;

/// <summary>
/// The lightest protocol available.
/// </summary>
public class AbridgedTransport(ITransport socket) : ITransport {
    public void Connect() {
        socket.Send(stackalloc byte[]{0xef});
    }
    public void Send(ReadOnlySpan<byte> packet) {
        if (packet.Length % 4 != 0)
            throw new ArgumentException("MTProto packet length must be a aligned to 4.", nameof(packet));
        
        var lengthInWords = packet.Length / 4;

        if (lengthInWords < 0x7f) {
            Span<byte> header = [(byte)lengthInWords];
            socket.Send(header);
        } else {
            // Wire format is always Little-Endian: 0x7F followed by 24-bit integer (lo, mid, hi)
            Span<byte> header = [
                0x7f,
                (byte)(lengthInWords & 0xFF),
                (byte)((lengthInWords >> 8) & 0xFF),
                (byte)((lengthInWords >> 16) & 0xFF)
            ];
            socket.Send(header);
        }

        socket.Send(packet);
    }
    public ReadOnlyMemory<byte> Receive() {
        Span<byte> lenMin = stackalloc byte[1];
        socket.ReceiveExact(lenMin);
        int totalLen = lenMin[0];
        if (totalLen == 0x7f) {
            var i = 0;
            Span<byte> lenBig = stackalloc byte[3];
            socket.ReceiveExact(lenBig);
            if (BitConverter.IsLittleEndian) {
                totalLen = (lenBig[i++] | lenBig[i++] << 8 | lenBig[i++] << 16);
            }
            else {
                totalLen = (lenBig[i++] << 16 | lenBig[i++] << 8 | lenBig[i++]);
            }
        }
        var payload = new byte[totalLen*4];
        socket.ReceiveExact(payload);
        if (totalLen == 1) {
            var errorCode = BinaryPrimitives.ReadInt32LittleEndian(payload);
            if (errorCode < 0)
                throw new IOException($"MTProto transport error: {errorCode}");
        }
        return payload;
    }

    public int ReceiveExact(Span<byte> packet) {
        throw new NotImplementedException();
    }
}