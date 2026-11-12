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

namespace NekoBot.Telegram.MTProto.AuthTypes.Server;

public class DhParamsOk : ITlBoxedType<DhParamsOk> {
    public static int Id => unchecked((int)0xd0e8075c);
    public Memory<byte> Nonce;
    public Memory<byte> ServerNonce;
    public TlString EncryptedAnswer;
    public int Serialize(Span<byte> destination) {
        return new TlLightWriter(destination)
            .WriteIdOf<DhParamsOk>()
            .WriteBytes(Nonce)
            .WriteBytes(ServerNonce)
            .WriteString(EncryptedAnswer)
            .Counter;
    }

    public int GetSerializedLength() => 4 + 16 + 16 + EncryptedAnswer.GetSerializedLength();
    
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