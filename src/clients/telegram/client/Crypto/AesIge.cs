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

using System.Runtime.Intrinsics;

namespace NekoBot.Telegram.Crypto;

public static class AesIge {
    /// <summary>
    /// AES-256-IGE hardware AES-NI instructions.
    /// </summary>
    public static byte[] Encrypt(ReadOnlySpan<byte> plainText, ReadOnlySpan<byte> key, ReadOnlySpan<byte> iv) {
        if (plainText.Length % 16 != 0)
            throw new ArgumentException("Plaintext must be a multiple of 16 bytes.");
        if (iv.Length != 32)
            throw new ArgumentException("IV must be 32 bytes (16 bytes IV1 and 16 bytes IV2).");
        
        var cipherText = new byte[plainText.Length];

        var cPrev = Vector128.Create(iv[..16]);
        var pPrev = Vector128.Create(iv.Slice(16, 16));

        // Use KeyExpansion to obtain round keys (or use Aes.Create() ECB mode if not expanding manually)
        using var aes = System.Security.Cryptography.Aes.Create();
        aes.Key = key.ToArray();
        aes.Mode = System.Security.Cryptography.CipherMode.ECB;
        aes.Padding = System.Security.Cryptography.PaddingMode.None;

        using var encryptor = aes.CreateEncryptor();
        var blockBuffer = new byte[16];
        var ecbOut = new byte[16];

        for (var i = 0; i < plainText.Length; i += 16) {
            var pBlock = Vector128.Create(plainText.Slice(i, 16));
            
            // P_i XOR C_{i-1}
            var xorIn = pBlock ^ cPrev;
            xorIn.CopyTo(blockBuffer);

            // Single block AES-ECB (hardware-backed by runtime)
            encryptor.TransformBlock(blockBuffer, 0, 16, ecbOut, 0);

            // C_i = AES(P_i XOR C_{i-1}) XOR P_{i-1}
            var aesRes = Vector128.Create(ecbOut);
            var cBlock = aesRes ^ pPrev;

            cBlock.CopyTo(cipherText.AsSpan(i, 16));

            pPrev = pBlock;
            cPrev = cBlock;
        }

        return cipherText;
    }
}