using System.Diagnostics;

namespace NekoBot.Adapter.Telegram;

public interface ITlType {
    /// <summary>
    /// Write an RPC Value of the type to the target stream.
    /// </summary>
    /// <param name="stream">A stream to write type to</param>
    /// <returns>Ammount written to the buffer.</returns>
    public abstract int Serialize(Span<byte> destination);
    /// <summary>
    /// Compute the total size of serialized value.
    /// </summary>
    public abstract int GetSerializedLength();

    /// <summary>
    /// Get the serialized value of this type.
    /// </summary>
    /// <returns>Serialized buffer.</returns>
    public byte[] Serialize() {
        var desiredLen = GetSerializedLength();
        var s = new byte[desiredLen];
        var actualWrittenLength = Serialize(s);
        Debug.Assert(desiredLen == actualWrittenLength); // nag user if the implementation is incorrect.
        return s;
    }

}