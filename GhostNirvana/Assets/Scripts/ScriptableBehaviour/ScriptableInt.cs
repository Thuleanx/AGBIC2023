using UnityEngine;

namespace ScriptableBehaviour {

[CreateAssetMenu(fileName = "Data",
                 menuName = "~/ScriptableFloat", order = 1)]
public class ScriptableInt : Scriptable<int> {}
}
