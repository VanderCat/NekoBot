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

using System.Security.Cryptography;
using NekoBot.Telegram.MTProto;
using NekoBot.Telegram.MTProto.AuthTypes.Server;
using TUnit.Assertions.Enums;

namespace NekoBot.Telegram.Tests;

public class DhTests {
    [Test]
    public async Task DeriveKeys() {
        var nonce = RandomNumberGenerator.GetBytes(16);
        var new_nonce = RandomNumberGenerator.GetBytes(32);
        var clientKeys = MTProtoSession.DeriveTemporaryKeys(new_nonce, nonce);
        var serverKeys = MockTelegramServer.DeriveTemporaryKeys(new_nonce, nonce);
        await Assert.That(clientKeys.Key).IsEquivalentTo(serverKeys.Key, CollectionOrdering.Matching);
        await Assert.That(clientKeys.Iv).IsEquivalentTo(serverKeys.Iv, CollectionOrdering.Matching);
    }
    [Test]
    public async Task TestDecryption() {
        var newNonce = RandomNumberGenerator.GetBytes(32);
        // answer := serialization
        var answer = new DhInnerData {
            ServerNonce = RandomNumberGenerator.GetBytes(16),
            Nonce =RandomNumberGenerator.GetBytes(16),
            G = 1,
            DhPrime = new TlString(RandomNumberGenerator.GetBytes(256)),
            Ga = new TlString(RandomNumberGenerator.GetBytes(4)),
            ServerTime = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        };
        var data = ((ITlType)answer).Serialize();
        var spanBuf = new byte[answer.GetSerializedLength()];
        answer.Serialize(spanBuf);
        await Assert.That(spanBuf).IsEquivalentTo(data, CollectionOrdering.Matching);
        var encrypted  = MockTelegramServer.EncryptDhInnerData(answer, newNonce);
        var decrypted = MTProtoSession.DecryptServerDhInnerData(encrypted, newNonce, answer.ServerNonce.Span);
        await Assert.That(decrypted.ServerNonce).IsEquivalentTo(answer.ServerNonce);
        await Assert.That(decrypted.Nonce).IsEquivalentTo(answer.Nonce);
        await Assert.That(decrypted.G).IsEqualTo(answer.G);
        await Assert.That(decrypted.DhPrime.Data).IsEquivalentTo(answer.DhPrime.Data, CollectionOrdering.Matching);
        await Assert.That(decrypted.Ga.Data).IsEquivalentTo(answer.Ga.Data, CollectionOrdering.Matching);
        await Assert.That(decrypted.ServerTime).IsEqualTo(answer.ServerTime);
    }
}