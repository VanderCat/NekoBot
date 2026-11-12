using NekoBot.Telegram;

namespace NekoBot.Adapter.Telegram.MTProto.AuthTypes.Server;

public class DhParamsOk : ITlBoxedType<DhParamsOk> {
    public static int Id => unchecked((int)0xd0e8075c);
    public Memory<byte> Nonce;
    public Memory<byte> ServerNonce;
    public TlString EncryptedAnswer;
    public int Serialize(Span<byte> destination) {
        throw new NotImplementedException();
    }
    public int GetSerializedLength() {
        throw new NotImplementedException();
    }
    
    public static DhParamsOk Deserialize(ReadOnlySpan<byte> data, out int bytesRead) {
        bytesRead = 0;
        var reader = new TlLightReader(data, ref bytesRead);
        reader.ThrowIfNotIdOf<DhParamsOk>();
        var res = new DhParamsOk {
            Nonce = reader.ConsumeBytes(16).ToArray(),
            ServerNonce = reader.ConsumeBytes(16).ToArray(),
            EncryptedAnswer = reader.ConsumeString()
        };
        return res;
    }
}