using UnityEngine;
using UnityEngine.UI;
using CombatSystem;

namespace GhostNirvana {

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
        if (_trackingStatus) {
            healthSlider.value = Mathf.Clamp01((float) _trackingStatus.Health / _trackingStatus.MaxHealth);
            transform.position = _trackingStatus.transform.position + Vector3.up * hoverDistance;
        }
    }
}

}
