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
using NekoBot.Telegram.MTProto.AuthTypes.Server;

namespace NekoBot.Telegram.Tests;

public class MockTelegramServer {
    /// <summary>
    /// poor quality code encryption of DhInnerData
    /// </summary>
    /// <param name="answer"></param>
    /// <param name="new_nonce"></param>
    /// <returns>encrypted answer</returns>
    public static byte[] EncryptDhInnerData(DhInnerData answer, ReadOnlySpan<byte> new_nonce) {
        // new_nonce_hash := 128 lower-order bits of SHA1 (new_nonce);
        var new_nonce_hash = SHA1.HashData(new_nonce)[4..20];

        // answer_with_hash := SHA1(answer) + answer + (0-15 random bytes); such that the length be divisible by 16;
        var sz = (int)(Math.Ceiling((float)(20 + answer.GetSerializedLength()) / 16) * 16);
        var answer_with_hash = new byte[sz];
        answer.Serialize(answer_with_hash.AsSpan(20..));
        SHA1.HashData(answer_with_hash.AsSpan(20, answer.GetSerializedLength()), answer_with_hash);
        var keys = DeriveTemporaryKeys(new_nonce, answer.ServerNonce.Span);
        // encrypted_answer := AES256_ige_encrypt (answer_with_hash, tmp_aes_key, tmp_aes_iv); here, tmp_aes_key is a 256-bit key, and tmp_aes_iv is a 256-bit initialization vector. The same as in all the other instances that use AES encryption, the encrypted data is padded with random bytes to a length divisible by 16 immediately prior to encryption.
        var encrypted_answer = Aes256Ige.Encrypt(answer_with_hash, keys.Key, keys.Iv);
        return encrypted_answer;
    }
    
    public static (byte[] Key, byte[] Iv) DeriveTemporaryKeys(ReadOnlySpan<byte> newNonce, ReadOnlySpan<byte> serverNonce) {
        // tmp_aes_key := SHA1(new_nonce + server_nonce) + substr (SHA1(server_nonce + new_nonce), 0, 12);
        var tmp_aes_key = SHA1.HashData(newNonce.ToArray().Concat(serverNonce.ToArray()).ToArray())!.Concat(SHA1.HashData(serverNonce.ToArray().Concat(newNonce.ToArray()).ToArray())[..12]).ToArray();
        // tmp_aes_iv := substr (SHA1(server_nonce + new_nonce), 12, 8) + SHA1(new_nonce + new_nonce) + substr (new_nonce, 0, 4);
        var tmp_aes_iv = SHA1.HashData(serverNonce.ToArray().Concat(newNonce.ToArray()).ToArray())[12..20]
            .Concat(SHA1.HashData(newNonce.ToArray().Concat(newNonce.ToArray()).ToArray()))
            .Concat(newNonce[..4].ToArray()).ToArray();
        return (tmp_aes_key, tmp_aes_iv);
    }
}