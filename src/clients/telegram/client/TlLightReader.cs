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

using System.Buffers.Binary;
using System.Runtime.CompilerServices;

namespace NekoBot.Telegram;

/// <summary>
/// Light reader, inlined as much as possible.
/// </summary>
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public ref struct TlLightReader(ReadOnlySpan<byte> buffer, ref int counter) {
    private ReadOnlySpan<byte> _buffer = buffer;
    private readonly ref int _counter = ref counter;
    
    public ReadOnlySpan<byte> RemaingBuffer {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _buffer[_counter..];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int ConsumeInt() {
        var result = BinaryPrimitives.ReadInt32LittleEndian(RemaingBuffer);
        _counter += 4;
        return result;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long ConsumeLong() {
        var result = BinaryPrimitives.ReadInt64LittleEndian(RemaingBuffer);
        _counter += 4;
        return result;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TlString ConsumeString() {
        var result = TlString.Deserialize(RemaingBuffer);
        _counter += result.GetSerializedLength();
        return result;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<byte> ConsumeBytes(int len) {
        var result = RemaingBuffer[..len];
        _counter += len;
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ThrowIfNotId(int expectedId) {
        var actualId = ConsumeInt();
        throw new InvalidDataException(
            $"Attempt to deserialize wrong type. Expected {expectedId:X}, Provided: {actualId:X}");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ThrowIfNotIdOf<T>() where T : ITlBoxedType => ThrowIfNotId(T.Id);
}