using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using NekoBot.Telegram;

namespace NekoBot.Adapter.Telegram;

/// <summary>
/// Light reader, inlined as much as possible.
/// </summary>
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public ref struct TlLightReader(ReadOnlySpan<byte> buffer, ref int counter) {
    private ReadOnlySpan<byte> _buffer = buffer;
    private readonly ref int _counter = ref counter;
    
    public ReadOnlySpan<byte> RemaingBuffer {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _buffer[_counter..];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int ConsumeInt() {
        var result = BinaryPrimitives.ReadInt32LittleEndian(RemaingBuffer);
        _counter += 4;
        return result;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long ConsumeLong() {
        var result = BinaryPrimitives.ReadInt64LittleEndian(RemaingBuffer);
        _counter += 4;
        return result;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TlString ConsumeString() {
        var result = TlString.Deserialize(RemaingBuffer);
        _counter += result.GetSerializedLength();
        return result;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<byte> ConsumeBytes(int len) {
        var result = RemaingBuffer[..len];
        _counter += len;
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ThrowIfNotId(int expectedId) {
        var actualId = ConsumeInt();
        throw new InvalidDataException(
            $"Attempt to deserialize wrong type. Expected {expectedId:X}, Provided: {actualId:X}");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ThrowIfNotIdOf<T>() where T : ITlBoxedType => ThrowIfNotId(T.Id);
}