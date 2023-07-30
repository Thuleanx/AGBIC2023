using UnityEngine;
using UnityEngine.UI;
using CombatSystem;
using ComposableBehaviour;

namespace GhostNirvana.UI {

public class StatusTracker : MonoBehaviour {
    Status _trackingStatus = null;

    public Status TrackingStatus { 
        get => _trackingStatus; 
        set {
            _trackingStatus = value;
            if (screenSpaceFollow && _trackingStatus)
                screenSpaceFollow.Target = _trackingStatus.Owner.transform;
        }
    }
    [SerializeField] ScreenSpaceFollow screenSpaceFollow;
    [SerializeField] Slider healthSlider;

    void LateUpdate() {
        if (!_trackingStatus) return;

        healthSlider.value = Mathf.Clamp01((float) _trackingStatus.Health / _trackingStatus.BaseStats.MaxHealth);
    }
}

}
