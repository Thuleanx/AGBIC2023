using UnityEngine;
using ScriptableBehaviour;

namespace GhostNirvana.Upgrade {

public class TimeKeeper : MonoBehaviour {
    [SerializeField] ScriptableFloat timeElapsedMinutes;

    protected void LateUpdate() {
        timeElapsedMinutes.Value += Time.deltaTime / 60.0f;
    }
}

}
