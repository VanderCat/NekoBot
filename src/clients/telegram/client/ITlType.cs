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

using System.Diagnostics;

namespace NekoBot.Telegram;

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