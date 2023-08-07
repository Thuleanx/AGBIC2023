using UnityEngine;
using UnityEngine.Events;

namespace ScriptableBehaviour {

[CreateAssetMenu(fileName = "Data",
                 menuName = "~/ScriptableFloat", order = 1)]
public class ScriptableFloat : Scriptable<float> {
}

public class Scriptable<T> : ScriptableObject {
    [field:System.NonSerialized]
    public T Value;
}

}
