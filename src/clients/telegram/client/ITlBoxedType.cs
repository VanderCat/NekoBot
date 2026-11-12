using System.Buffers.Binary;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using NekoBot.Adapter.Telegram;

namespace NekoBot.Telegram;

// NOTE: warcrimes
//       i have no idea if this thing is work but it feels to make a contract to force on all implementors a static
//       function that implements te deserialization of the type a little bit forbidden and that should not be done.
//       i however, do not care and this seems fun. - Kot

public interface ITlBoxedType : ITlType {
    /// <summary>
    /// The TL id for this type.
    /// </summary>
    public static abstract int Id { get; }
}

public interface ITlBoxedType<out T> : ITlBoxedType where T : ITlBoxedType {
    /// <summary>
    /// Deserialize the constructor of boxed type.
    /// </summary>
    /// <param name="data">Data containing serialized value.</param>
    /// <param name="bytesRead">Amount of data was read to construct a value.</param>
    /// <returns>Value serialized in data.</returns>
    public new static abstract T Deserialize(ReadOnlySpan<byte> data, out int bytesRead);
    
    /// <summary>
    /// Check if the span contains id for this boxed type.
    /// </summary>
    /// <param name="data"></param>
    /// <exception cref="InvalidDataException">id mismatch</exception>
    protected static void CheckId(ReadOnlySpan<byte> data) {
        var expectedId = T.Id;
        var actualId = BinaryPrimitives.ReadInt32LittleEndian(data);
        if (expectedId != actualId)
            throw new InvalidDataException(
                $"Attempt to deserialize wrong type. Expected {expectedId:X}, Provided: {actualId:X}");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected void WriteId(Span<byte> data) => BinaryPrimitives.WriteInt32LittleEndian(data, T.Id);
}

/// <summary>
/// Helper record that provides implementation of boxed type
/// </summary>
/// <typeparam name="T">A type with a constructor with no arguments.</typeparam>
public abstract record TlSimpleBoxedType<T> : ITlBoxedType<T> where T : ITlBoxedType, new() {
    public static T Deserialize(ReadOnlySpan<byte> data, out int bytesRead) {
        ITlBoxedType<T>.CheckId(data);
        bytesRead = 4;
        return new T();
    }
    
    public int Serialize(Span<byte> destination) {
        Debug.Assert(destination.Length >= 4);
        BinaryPrimitives.WriteInt32LittleEndian(destination, Id);
        return 4;
    }

    public int GetSerializedLength() => 4;

    public static int Id => T.Id;
    
    static T ITlBoxedType<T>.Deserialize(ReadOnlySpan<byte> data, out int bytesRead) => Deserialize(data, out bytesRead);
}