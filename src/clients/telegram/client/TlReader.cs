using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace NekoBot.Adapter.Telegram;

public partial class TlReader(ReadOnlyMemory<byte> payload) {
    public int Position;
    private readonly ReadOnlyMemory<byte> _payload = payload;
    private bool IsAligned => Position % 4 == 0;

    /// <summary>
    /// Get current byte and advance to next byte. Unaligned.
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private byte ConsumeByte() => _payload.Span[Position++];

    /// <summary>
    /// See the next byte without moving the cursor.
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private byte PeekByte() => _payload.Span[Position + 1];

    /// <summary>
    /// See the byte cursor currently is on.
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private byte CurrentByte() => _payload.Span[Position];


    /// <summary>
    /// Read primitive
    /// </summary>
    /// <typeparam name="T">any base type</typeparam>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private unsafe T Consume<T>() where T: unmanaged {
        Debug.Assert(BitConverter.IsLittleEndian);
        return MemoryMarshal.Read<T>(ReadRange(sizeof(T)));
    }

    /// <summary>
    /// Read range.
    /// </summary>
    /// <param name="size"></param>
    /// <returns>Span with specified length</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<byte> ReadRange(int size) {
        Debug.Assert(Position + size <= _payload.Length);
        var result = _payload.Span[Position..(Position + size)];
        Position += size;
        return result;
    }

    /// <summary>
    /// Read range with alignment
    /// </summary>
    /// <param name="size"></param>
    /// <returns>Span with specified length, e.g. for strings</returns>
    /// <remarks>
    /// Will automatically skip bytes that arent aligned, e.g. for reading string
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<byte> ReadRangeAligned(int size) {
        var range = ReadRange(size);
        while (!IsAligned) {
            ConsumeByte();
        }
        return range;
    }

    /// <summary>
    /// Consume UInt with alignment check.
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint ReadAlignedUnsigned() {
        Debug.Assert(IsAligned);
        return Consume<uint>();
    }
    /// <summary>
    /// Consume Int with alignment check.
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int ReadAlignedSigned() {
        Debug.Assert(IsAligned);
        return Consume<int>();
    }

    /// <summary>
    /// Read TL string length from Span.
    /// </summary>
    /// <param name="bytes">must be atleast one.</param>
    /// <returns></returns>
    /// <exception cref="FormatException"></exception>
    [Obsolete("Use TlString.ReadLength")]
    public static int ReadStringLength(ReadOnlySpan<byte> bytes) => TlString.ReadLength(bytes);

    /// <summary>
    /// Read TL string length.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="FormatException"></exception>
    /// <remarks>
    /// If length are less than 253 then reading position is unaligned! <see cref="IsAligned"/>
    /// </remarks>
    public int ReadStringLength() {
        var first = ConsumeByte();
        if (first <= 253) {
            return first;
        }
        if (first == 254) {
            return ConsumeByte() | (ConsumeByte() << 8) | (ConsumeByte() << 16);
        } 
        throw new FormatException("Invalid TL length byte (255)");
    }

    /// <summary>
    /// Read TL string. Aligned.
    /// </summary>
    /// <returns>Tl String</returns>
    public TlString ReadString() {
        var len = ReadStringLength();
        var str = new TlString(ReadRangeAligned(len));
        Debug.Assert(IsAligned);
        return str;
    }
    

    /// <summary>
    /// Read vector of unboxed types.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T[] ReadVector<T>() where T: unmanaged {
        var id = ReadAlignedUnsigned();
        if (id != 0x1cb5c415)
            throw new InvalidDataException("The specified id is not of an Vector");
        var count = ReadAlignedUnsigned();
        var value = new T[count];
        for (uint i = 0; i < count; i++) {
            value[i] = Consume<T>();
            while (!IsAligned) {
                ConsumeByte();
            }
        }
        return value;
    }
}