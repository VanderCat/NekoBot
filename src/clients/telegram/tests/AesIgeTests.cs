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
using NekoBot.Telegram.Crypto;
using TUnit.Assertions.Enums;

namespace NekoBot.Telegram.Tests;

public class AesIgeTests {
    [Test]
    [Arguments(16)]
    [Arguments(32)]
    [Arguments(8192)]
    public async Task SameAfterDecryption(int size) {
        Span<byte> iv = stackalloc byte[32];
        Span<byte> key = stackalloc byte[32];
        var data = new byte[size];
        RandomNumberGenerator.Fill(iv);
        RandomNumberGenerator.Fill(key);
        RandomNumberGenerator.Fill(data);
        var encrypted = AesIge.Encrypt(data, key, iv);
        var decrypted = AesIge.Decrypt(encrypted, key, iv);
        await Assert.That(decrypted).IsNotEmpty();
        await Assert.That(decrypted).Count().IsEqualTo(size);
        await Assert.That(decrypted).IsEquivalentTo(data, CollectionOrdering.Matching);
    }
}