using UnityEngine;
using ScriptableBehaviour;

namespace ComposableBehaviour {

public class ScaleByScriptableFloat : MonoBehaviour {
    [SerializeField] ScriptableFloat scaleFloat;

    void Update() {
        transform.localScale = scaleFloat.Value * Vector3.one;
    }
}

}
