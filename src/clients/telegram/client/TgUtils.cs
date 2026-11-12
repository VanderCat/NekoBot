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
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NekoBot.Telegram;

internal static class TgUtils {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<byte> GetAsSpan<T>(ref T array) where T: unmanaged =>
        MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref array, 1));
    
    /// <summary>
    /// Convert to value endian string.
    /// </summary>
    /// <param name="value">Value to convert.</param>
    /// <returns>Big endian string.</returns>
    /// <remarks>
    /// Telegram expects p and q values while negotiating an encryption connection in big endian format, as it is passed
    /// on server backed to openssl directly.
    /// </remarks>
    public static byte[] ToBigEndianBytes(ulong value) {
        var bigEndian = BitConverter.IsLittleEndian ? BinaryPrimitives.ReverseEndianness(value) : value;
        return GetAsSpan(ref bigEndian).ToArray(); //copy
    }
    public static byte[] ToBigEndianBytes(uint value) {
        var bigEndian = BitConverter.IsLittleEndian ? BinaryPrimitives.ReverseEndianness(value) : value;
        return GetAsSpan(ref bigEndian).ToArray(); //copy
    }
}