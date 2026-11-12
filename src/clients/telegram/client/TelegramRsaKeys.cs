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

using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using NekoBot.Telegram.Crypto;

namespace NekoBot.Telegram;

public struct TelegramRsaKey {
    public ulong Fingerprint;
    public RSA Rsa;

    public TelegramRsaKey(ulong fingerprint, ReadOnlySpan<char> pem) {
        Fingerprint = fingerprint;
        Rsa = RSA.Create();
        Rsa.ImportFromPem(pem);
    }
    public TelegramRsaKey(ReadOnlySpan<char> pem) {
        Rsa = RSA.Create();
        Rsa.ImportFromPem(pem);
        Fingerprint = Rsa.ComputeTelegramFingerprint();
    }
}

public class TelegramRsaKeys {
    private static readonly TelegramRsaKey[] AllKeys = [
        new(
            """
             -----BEGIN RSA PUBLIC KEY-----
             MIIBCgKCAQEA6LszBcC1LGzyr992NzE0ieY+BSaOW622Aa9Bd4ZHLl+TuFQ4lo4g
             5nKaMBwK/BIb9xUfg0Q29/2mgIR6Zr9krM7HjuIcCzFvDtr+L0GQjae9H0pRB2OO
             62cECs5HKhT5DZ98K33vmWiLowc621dQuwKWSQKjWf50XYFw42h21P2KXUGyp2y/
             +aEyZ+uVgLLQbRA1dEjSDZ2iGRy12Mk5gpYc397aYp438fsJoHIgJ2lgMv5h7WY9
             t6N/byY9Nw9p21Og3AoXSL2q/2IJ1WRUhebgAdGVMlV1fkuOQoEzR7EdpqtQD9Cs
             5+bfo3Nhmcyvk5ftB0WkJ9z6bNZ7yxrP8wIDAQAB
             -----END RSA PUBLIC KEY-----
             """
        ),
    ];
    
    public static readonly FrozenDictionary<ulong, TelegramRsaKey> ByFingerprint =
        AllKeys.ToFrozenDictionary(k => k.Fingerprint);
    
    public static bool TryFindKey(ulong serverFingerprint, [MaybeNullWhen(false)] out TelegramRsaKey? matchedKey) {
        var r = ByFingerprint.TryGetValue(serverFingerprint, out var v);
        if (r) {
            matchedKey = v;
            return true;
        }
        matchedKey = null;
        return false;
    }

    public static bool TryFindKey(IEnumerable<ulong> serverFingerprints, [MaybeNullWhen(false)] out TelegramRsaKey? matchedKey) {
        foreach (long fp in serverFingerprints) 
            if (TryFindKey((ulong)fp, out matchedKey))
                return true;
            
        matchedKey = null;
        return false;
    }
}