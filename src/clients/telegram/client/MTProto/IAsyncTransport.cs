// Copyright 2026 VanderCat

// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at

//     http://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace NekoBot.Telegram.MTProto;

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