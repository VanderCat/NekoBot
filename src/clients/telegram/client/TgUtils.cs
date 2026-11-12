using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NekoBot.Adapter.Telegram;

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