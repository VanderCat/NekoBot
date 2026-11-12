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

namespace NekoBot.Telegram;

// public record TlVector<T> : ITlBoxedType, IList<T> where T: ITlType {
//     private List<T> _items;
//     
//
//     public IEnumerator<T> GetEnumerator() => _items.GetEnumerator();
//
//     IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
//     public override int Id => 481674261;
//     public bool Remove(T item) => _items.Remove(item);
//     public int Count => _items.Count;
//     public bool IsSynchronized => false;
//     public object SyncRoot => new object();
//     public void Add(T item) => _items.Add(item);
//
//     public void Clear() => _items.Clear();
//
//     public bool Contains(T item) => _items.Contains(item);
//     public void CopyTo(T[] array, int arrayIndex) => _items.CopyTo(array, arrayIndex);
//
//     public int IndexOf(T item) => _items.IndexOf(item);
//     public void Insert(int index, T item) => _items.Insert(index, item);
//
//     public void RemoveAt(int index) => _items.RemoveAt(index);
//
//     public T this[int index] {
//         get => _items[index];
//         set => _items[index] = value;
//     }
//
//     public bool IsFixedSize => false;
//     public bool IsReadOnly => false;
//
//     public override void Serialize(Span<byte> destination) {
//         throw new NotImplementedException();
//     }
//     public override void Deserialize(ReadOnlySpan<byte> data) {
//         throw new NotImplementedException();
//     }
// }