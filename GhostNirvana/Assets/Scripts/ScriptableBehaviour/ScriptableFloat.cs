using UnityEngine;

namespace ScriptableBehaviour {

public class ScriptableFloat : ScriptableObject {
    [field:System.NonSerialized]
    public float Value;
}

public class Scriptable<T> : ScriptableObject {
    [field:System.NonSerialized]
    public T Value;
}

}
