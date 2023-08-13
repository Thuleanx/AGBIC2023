using UnityEngine;
using ScriptableBehaviour;
using NaughtyAttributes;

namespace GhostNirvana.Upgrade {

public class TimeKeeper : MonoBehaviour {
    [SerializeField] ScriptableFloat timeElapsedMinutes;
    [SerializeField] bool active = false;

    protected void Awake() {
        timeElapsedMinutes.Value = 0;
    }

    [Button] public void Activate() => active = true;

    protected void LateUpdate() {
        if (active) timeElapsedMinutes.Value += Time.deltaTime / 60.0f;
    }
}

}
