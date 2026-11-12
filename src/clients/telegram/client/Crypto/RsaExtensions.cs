using System.Buffers.Binary;
using System.Numerics;
using System.Security.Cryptography;

namespace NekoBot.Adapter.Telegram.Crypto;

public static class RsaExtensions {
    /// <summary>
    /// Encrypt the input data using Telegram's RSA padding.
    /// </summary>
    /// <param name="rsa">RSA public key.</param>
    /// <param name="data">The data to encrypt.</param>
    /// <returns></returns>
    public static byte[] EncryptRsaPad(this RSA rsa, ReadOnlySpan<byte> data) {
        ArgumentNullException.ThrowIfNull(rsa);
        var rsaParams = rsa.ExportParameters(false);
        return RsaPad(data, rsaParams.Modulus!, rsaParams.Exponent!);
    }

    /// <summary>
    /// MTProto RSA_PAD
    /// </summary>
    /// <param name="data">TL-serialized data to encrypt (max 144 bytes).</param>
    /// <param name="rsaModulus">RSA public modulus (256-byte unsigned big-endian array).</param>
    /// <param name="rsaExponent">RSA public exponent (unsigned big-endian array, commonly e = 65537).</param>
    /// <returns>256-byte encrypted output.</returns>
    public static byte[] RsaPad(ReadOnlySpan<byte> data, ReadOnlySpan<byte> rsaModulus, ReadOnlySpan<byte> rsaExponent) {
        if (data.Length > 144)
            throw new ArgumentException("Data must not exceed 144 bytes.", nameof(data));
        if (rsaModulus.Length != 256)
            throw new ArgumentException("Modulus must be exactly 256 bytes (2048-bit).", nameof(rsaModulus));
        
        var modulus = new BigInteger(rsaModulus, isUnsigned: true, isBigEndian: true);
        var exponent = new BigInteger(rsaExponent, isUnsigned: true, isBigEndian: true);

        // 1. data_with_padding := data + random_padding_bytes (length = 192 bytes)
        var dataWithPadding = new byte[192];
        data.CopyTo(dataWithPadding);
        RandomNumberGenerator.Fill(dataWithPadding.AsSpan(data.Length));

        // 2. data_pad_reversed := BYTE_REVERSE(data_with_padding)
        var dataPadReversed = dataWithPadding.Reverse().ToArray();

        Span<byte> tempKey = stackalloc byte[32];
        var keyAesEncrypted = new byte[256];
        Span<byte> zeroIv = stackalloc byte[32]; // Zero IV for 256-IGE is 32 bytes (16 bytes IV1 + 16 bytes IV2)
        
        var dataSize = dataWithPadding.Length + 32;
        
        Span<byte> hash = stackalloc byte[32];
        Span<byte> tempKeyXor = stackalloc byte[32];
        
        var sha256Payload = new byte[dataSize];
        var sha256PayloadSpan = sha256Payload.AsSpan();
        
        var dataWithHash = new byte[dataSize];
        var dataWithHashSpan = dataWithHash.AsSpan();
        
        var encryptedData = new byte[256];
        var padded = new byte[256];
        
        while (true) {
            // 3. Generate random 32-byte temp_key
            RandomNumberGenerator.Fill(tempKey);

            // 4. data_with_hash := data_pad_reversed + SHA256(temp_key + data_with_padding) -> 224 bytes
            tempKey.CopyTo(sha256PayloadSpan[..32]);
            dataWithPadding.CopyTo(sha256PayloadSpan[32..]);
            SHA256.HashData(sha256Payload, hash);
            
            dataPadReversed.CopyTo(dataWithHashSpan);
            hash.CopyTo(dataWithHashSpan[dataPadReversed.Length..]);

            // 5. aes_encrypted := AES256_IGE(data_with_hash, temp_key, 0)
            var aesEncrypted = AesIge.Encrypt(dataWithHash, tempKey, zeroIv); //TODO: zero allocations inside loops

            // 6. temp_key_xor := temp_key XOR SHA256(aes_encrypted)
            SHA256.HashData(aesEncrypted, hash);
            
            for (int i = 0; i < 32; i++)
                tempKeyXor[i] = (byte)(tempKey[i] ^ hash[i]);

            // 7. key_aes_encrypted := temp_key_xor + aes_encrypted -> 256 bytes
            tempKeyXor.CopyTo(keyAesEncrypted);
            aesEncrypted.CopyTo(keyAesEncrypted.AsSpan(32));

            // 8. Compare key_aes_encrypted with RSA modulus as unsigned big-endian integer
            var mValue = new BigInteger(keyAesEncrypted, isUnsigned: true, isBigEndian: true);
            if (mValue >= modulus) continue;
            
            // 9. encrypted_data := RSA(key_aes_encrypted, server_pubkey)
            // c = m^e mod N
            var cValue = BigInteger.ModPow(mValue, exponent, modulus);

            // Convert to exactly 256 bytes (big-endian, zero-padded if necessary)
                
            if (!cValue.TryWriteBytes(encryptedData, out var bytesWritten, isUnsigned: true, isBigEndian: true)) {
                throw new InvalidOperationException("Failed to write RSA output bytes.");
            }

            // If fewer than 256 bytes were written, shift right to ensure leading zeros
            if (bytesWritten < 256) {
                encryptedData.CopyTo(padded.AsSpan(256 - bytesWritten));
                Buffer.BlockCopy(encryptedData, 0, padded, 256 - bytesWritten, bytesWritten);
                return padded;
            }

            return encryptedData;
        }
    }
    
    public static ulong ComputeTelegramFingerprint(this RSA rsa) {
        var p = rsa.ExportParameters(false);
        var nStr = new TlString(p.Modulus!);
        var eStr = new TlString(p.Exponent!);
        Span<byte> buf = stackalloc byte[nStr.GetSerializedLength() + eStr.GetSerializedLength()];
        var offset = nStr.Serialize(buf);
        eStr.Serialize(buf[offset..]);
        
        Span<byte> sha1 = stackalloc byte[20];
        SHA1.HashData(buf, sha1);
        return BinaryPrimitives.ReadUInt64LittleEndian(sha1.Slice(12, 8));
    }
}