using UnityEngine;

namespace ScriptableBehaviour {
[CreateAssetMenu(fileName = "GameObject",
                 menuName = "~/Sets/GameObject", order = 1)]
public class GameObjectRuntimeSet : RuntimeSet<GameObject> { }
}
