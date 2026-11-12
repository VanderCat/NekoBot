using System.Collections;

namespace NekoBot.Adapter.Telegram;

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