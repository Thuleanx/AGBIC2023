using UnityEngine;
using ScriptableBehaviour;

namespace GhostNirvana.Upgrade {

public class TimeKeeper : MonoBehaviour {
    [SerializeField] ScriptableFloat timeElapsedMinutes;

    protected void Awake() {
        timeElapsedMinutes.Value = 0;
    }

    protected void LateUpdate() {
        timeElapsedMinutes.Value += Time.deltaTime / 60.0f;
    }
}

}
