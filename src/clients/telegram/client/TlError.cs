using System.Buffers.Binary;
using NekoBot.Telegram;

namespace NekoBot.Adapter.Telegram;

public record TlError(int Code, TlString Text) : ITlBoxedType<TlError> {
    public static int Id => unchecked((int)0xc4b9f9bb);
    
    public int Serialize(Span<byte> destination) {
        throw new NotImplementedException();
    }

    public int GetSerializedLength() => 4 + 4 + Text.GetSerializedLength();

    public static TlError Deserialize(ReadOnlySpan<byte> data, out int read) {
        read = 0;
        ITlBoxedType<TlError>.CheckId(data); // warcrimes
        read += 4;
        var code = BinaryPrimitives.ReadInt32LittleEndian(data[read..]);
        read += 4;
        var str = TlString.Deserialize(data[read..]);
        return new TlError(code, str);
    }
}