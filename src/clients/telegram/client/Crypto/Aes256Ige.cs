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

using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Security.Cryptography;

namespace NekoBot.Telegram.Crypto;

public static class Aes256Ige {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ValidateInputs(ReadOnlySpan<byte> plainText, ReadOnlySpan<byte> key, ReadOnlySpan<byte> iv) {
        if (plainText.Length % 16 != 0)
            throw new ArgumentException("Plaintext must be a multiple of 16 bytes.", nameof(plainText));
        if (iv.Length != 32)
            throw new ArgumentException("IV must be 32 bytes (16 bytes IV1 and 16 bytes IV2).", nameof(iv));
        if (key.Length != 32)
            throw new ArgumentException("AES-256 key must be exactly 32 bytes (256 bits).", nameof(key));
    }

    /// <summary>
    /// AES-256-IGE hardware AES-NI instructions.
    /// </summary>
    public static void Encrypt(ReadOnlySpan<byte> plainText, ReadOnlySpan<byte> key, ReadOnlySpan<byte> iv, Span<byte> cipherText) {
        ValidateInputs(plainText, key, iv);
        if (cipherText.Length < plainText.Length)
            throw new ArgumentException("Provided destination can't fit encrypted text.", nameof(cipherText));
        
        using var aes = Aes.Create();
        aes.Key = key.ToArray();

        var cPrev = Vector128.Create(iv[..16]);
        var pPrev = Vector128.Create(iv[16..32]);
        
        Span<byte> blockIn = stackalloc byte[16];
        Span<byte> blockOut = stackalloc byte[16];

        for (var i = 0; i < plainText.Length; i += 16) {
            var pBlock = Vector128.Create(plainText.Slice(i, 16));
            
            // P_i XOR C_{i-1}
            var xorIn = pBlock ^ cPrev;
            xorIn.CopyTo(blockIn);

            // Single block AES-ECB (hardware-backed by runtime)
            aes.EncryptEcb(blockIn, blockOut, PaddingMode.None);

            // C_i = AES(P_i XOR C_{i-1}) XOR P_{i-1}
            var aesRes = Vector128.Create(blockOut);
            var cBlock = aesRes ^ pPrev;

            cBlock.CopyTo(cipherText.Slice(i, 16));

            pPrev = pBlock;
            cPrev = cBlock;
        }
    }

    public static byte[] Encrypt(ReadOnlySpan<byte> plainText, ReadOnlySpan<byte> key, ReadOnlySpan<byte> iv) {
        var cipherText = new byte[plainText.Length];
        Encrypt(plainText, key, iv, cipherText);
        return cipherText;
    }
    
    public static void Decrypt(ReadOnlySpan<byte> cipherText, ReadOnlySpan<byte> key, ReadOnlySpan<byte> iv, Span<byte> plaintext) {
        ValidateInputs(cipherText, key, iv);
        if (plaintext.Length < cipherText.Length)
            throw new ArgumentException("Provided destination can't fit decrypted text.", nameof(plaintext));

        using var aes = Aes.Create();
        aes.Key = key.ToArray();

        var cPrev = Vector128.Create(iv[..16]);  // C_{-1}
        var pPrev = Vector128.Create(iv[16..32]); // P_{-1}

        Span<byte> blockIn = stackalloc byte[16];
        Span<byte> blockOut = stackalloc byte[16];

        for (var i = 0; i < cipherText.Length; i += 16) {
            var cCurr = Vector128.Create(cipherText.Slice(i, 16)); // C_i
            var xorIn = cCurr ^ pPrev; // C_i ^ P_{i-1}
            xorIn.CopyTo(blockIn);
            
            // blockOut = AES_DEC(blockIn)
            aes.DecryptEcb(blockIn, blockOut, PaddingMode.None);

            // P_i = blockOut ^ C_{i-1}
            var aesRes = Vector128.Create(blockOut);
            var pCurr = aesRes ^ cPrev;

            pCurr.CopyTo(plaintext.Slice(i, 16));

            pPrev = pCurr;
            cPrev = cCurr;
        }
    }
    public static byte[] Decrypt(ReadOnlySpan<byte> cipherText, ReadOnlySpan<byte> key, ReadOnlySpan<byte> iv) {
        var plainText = new byte[cipherText.Length];
        Decrypt(plainText, key, iv, plainText);
        return plainText;
    }
}