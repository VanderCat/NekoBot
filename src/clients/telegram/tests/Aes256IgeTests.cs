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

public class Aes256IgeTests {
    [Test]
    [Arguments(16)]
    [Arguments(32)]
    [Arguments(8192)]
    public async Task SameAfterDecryption(int size) {
        Span<byte> iv = stackalloc byte[32];
        Span<byte> key = stackalloc byte[32];
        var data = new byte[size];
        var encryptionBuffer = new byte[size];
        RandomNumberGenerator.Fill(iv);
        RandomNumberGenerator.Fill(key);
        RandomNumberGenerator.Fill(data);
        Aes256Ige.Encrypt(data, key, iv, encryptionBuffer);
        Aes256Ige.Decrypt(encryptionBuffer, key, iv, encryptionBuffer);
        await Assert.That(encryptionBuffer).IsNotEmpty();
        await Assert.That(encryptionBuffer).Count().IsEqualTo(size);
        await Assert.That(encryptionBuffer).IsEquivalentTo(data, CollectionOrdering.Matching);
    }
}