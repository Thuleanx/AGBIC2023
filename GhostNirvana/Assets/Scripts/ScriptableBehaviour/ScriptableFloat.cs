using UnityEngine;

namespace ScriptableBehaviour {

[CreateAssetMenu(fileName = "Data",
                 menuName = "~/ScriptableFloat", order = 1)]
public class ScriptableFloat : ScriptableObject {
    [field:System.NonSerialized]
    public float Value;
}

public class Scriptable<T> : ScriptableObject {
    [field:System.NonSerialized]
    public T Value;
}

}
