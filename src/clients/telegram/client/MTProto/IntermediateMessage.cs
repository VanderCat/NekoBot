using System.Text;

namespace NekoBot.Adapter.Telegram.MTProto;

public record IntermediateMessage(byte[] payload) {
    public void Write(Stream stream) {
        using var writer = new BinaryWriter(stream, Encoding.UTF8, true);
        var size = (uint)(payload.Length / 4);
        writer.Write(size);
        writer.Write(payload);
    }
}