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

using System.Diagnostics;
using System.Text;

namespace NekoBot.Telegram;

public class TlString : ITlType {
    public TlString(byte[] data) {
        Data = data;
        if (Data.Length > 0xFFFFFF)
            throw new InvalidOperationException("String too long");
    }
    public TlString(ReadOnlySpan<byte> data) : this(data.ToArray()) { }

    public TlString(string str) : this(Encoding.UTF8.GetBytes(str)) { }

    public byte[] Data;
    public int Length => Data.Length;

    [Conditional("DEBUG")]
    private void AssertStringSize(int dstLen) {
        var lengthNeeded = GetSerializedLength();
        Debug.Assert(dstLen >= lengthNeeded);
    }
    
    public int Serialize(Span<byte> destination) {
        var len = Length;
        var offset = 0;
        AssertStringSize(destination.Length);
        if (len <= 253) {
            destination[0] = (byte)len;
            offset = 1;
        } else {
            destination[0] = 0xFE; // special byte
            destination[1] = unchecked((byte)(len & 0xFF)); // TODO: should be this changed on big endian platforms?
            destination[2] = unchecked((byte)((len >> 8) & 0xFF));
            destination[3] = unchecked((byte)((len >> 16) & 0xFF));
            offset = 4;
        }
        
        Data.CopyTo(destination[offset..]);
        var padding = (4 - (int)((len + offset) % 4)) % 4;
        for (var i = 0; i < padding; i++)
            destination[offset+len+i] = 0x00; //zero
        Debug.Assert((len + offset + padding) % 4 == 0); // should be aligned
        return len + offset + padding;
    }

    /// <summary>
    /// Read TL string length from Span.
    /// </summary>
    /// <param name="bytes">must be atleast one.</param>
    /// <returns></returns>
    /// <exception cref="FormatException"></exception>
    public static int ReadLength(ReadOnlySpan<byte> bytes) {
        Debug.Assert(bytes.Length == 1 && bytes[0] < 254);
        Debug.Assert(bytes.Length > 4 && bytes[0] == 254);
        var first = bytes[0];
        if (first <= 253) {
            return first;
        }
        if (first == 254) {
            return bytes[1] | (bytes[2] << 8) | (bytes[3] << 16);
        }
        throw new FormatException("Invalid TL length byte (255)");
    }

    public static TlString Deserialize(ReadOnlySpan<byte> buffer) {
        var len = ReadLength(buffer);
        ReadOnlySpan<byte> actualString;
        if (len <= 253) {
            actualString = buffer[1..len];
        }
        else {
            actualString = buffer[4..len];
        }

        return new TlString(actualString);
    }

    public int GetSerializedLength() {
        var size = Length;
        if (size <= 253) {
            // the serialization contains one byte with the value of L, then L bytes of the string followed by zero padding. 
            size += 1;
        }
        else {
            size += 4;
        }

        size += (4 - size % 4) % 4;

        return size;
    }

    public override string ToString() {
        return Encoding.UTF8.GetString(Data);
    }
    
    public static implicit operator Span<byte>(TlString d) => d.Data.AsSpan();
    public static implicit operator ReadOnlySpan<byte>(TlString d) => d.Data.AsSpan();
    public static explicit operator TlString(string s) => new(s);
}