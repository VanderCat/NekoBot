namespace NekoBot.Adapter.Telegram.MTProto;

public interface ITransport {
    /// <summary>
    /// Connect to the server using handshake.
    /// </summary>
    /// <returns></returns>
    void Connect();
    /// <summary>
    /// Send packet.
    /// </summary>
    /// <param name="packet"></param>
    /// <returns></returns>
    void Send(ReadOnlySpan<byte> packet);
    /// <summary>
    /// Receive response.
    /// </summary>
    /// <returns></returns>
    ReadOnlyMemory<byte> Receive();

    /// <summary>
    /// Receive response.
    /// </summary>
    /// <param name="packet">a span with a defined length to read</param>
    /// <returns>Ammount of bytes read</returns>
    int ReceiveExact(Span<byte> packet);
    
    int ReceiveExact(byte[] packet, int offset, int count) => ReceiveExact(packet.AsSpan(offset..(offset + count)));
}