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

//TODO: While this happens to work right now, returning this by value in a mutable ref struct is a ticking time bomb.
/// <summary>
/// Light writer, inlined as much as possible.
/// </summary>
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public ref struct TlLightWriter(Span<byte> buffer) {
    private Span<byte> _buffer = buffer;
    private int _counter = 0;
    public int Counter {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _counter;
    }

    public Span<byte> RemaingBuffer {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _buffer[_counter..];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TlLightWriter WriteInt(int value) {
        BinaryPrimitives.WriteInt32LittleEndian(RemaingBuffer, value);
        _counter += 4;
        return this;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TlLightWriter WriteLong(long value) {
        BinaryPrimitives.WriteInt64LittleEndian(RemaingBuffer, value);
        _counter += 8;
        return this;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TlLightWriter WriteString(TlString tlString) {
        _counter += tlString.Serialize(RemaingBuffer);
        return this;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TlLightWriter WriteBytes(ReadOnlySpan<byte> span) {
        span.CopyTo(RemaingBuffer);
        _counter += span.Length;
        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TlLightWriter WriteBytes(ReadOnlyMemory<byte> mem) => WriteBytes(mem.Span);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TlLightWriter WriteIdOf<T>() where T : ITlBoxedType => WriteInt(T.Id);
}