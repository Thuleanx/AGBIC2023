using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScriptableBehaviour {

public class RuntimeSet<T> : ScriptableObject, ISerializationCallbackReceiver, IEnumerable<T> {
    [System.NonSerialized]
    HashSet<T> collection = new HashSet<T>();

    public void Add(T item) => collection.Add(item);
    public void Remove(T item) => collection.Remove(item);

    public void OnAfterDeserialize() => collection.Clear();
    public void OnBeforeSerialize() => collection.Clear();

    public IEnumerator<T> GetEnumerator() => collection.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }
}

}
