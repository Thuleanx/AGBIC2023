using UnityEngine;
using UnityEngine.UI;
using CombatSystem;

namespace GhostNirvana.UI {

public class StatusTracker : MonoBehaviour {
    Status _trackingStatus = null;

    public Status TrackingStatus { 
        get => _trackingStatus; 
        set {
            _trackingStatus = value;
        }
    }

    [SerializeField] float hoverDistance = 2;
    [SerializeField] Slider healthSlider;

    void LateUpdate() {
        if (_trackingStatus != null) {
            healthSlider.value = Mathf.Clamp01((float) _trackingStatus.Health / _trackingStatus.BaseStats.MaxHealth);
            transform.position = _trackingStatus.Owner.transform.position + Vector3.up * hoverDistance;
        }
    }
}

}
