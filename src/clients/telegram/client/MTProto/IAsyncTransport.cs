namespace NekoBot.Adapter.Telegram.MTProto;

public interface IAsyncTransport {
    /// <summary>
    /// Connect to the Telegram server
    /// </summary>
    /// <param name="ct"></param>
    /// <returns></returns>
    ValueTask ConnectAsync(CancellationToken ct);
    /// <summary>
    /// Send neatly packaged message to Telegram server
    /// </summary>
    /// <param name="packet"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    ValueTask SendAsync(ReadOnlyMemory<byte> packet, CancellationToken ct);
    /// <summary>
    /// Receive unpacked message
    /// </summary>
    /// <param name="ct"></param>
    /// <returns></returns>
    ValueTask<ReadOnlyMemory<byte>> ReceiveAsync(CancellationToken ct);
}