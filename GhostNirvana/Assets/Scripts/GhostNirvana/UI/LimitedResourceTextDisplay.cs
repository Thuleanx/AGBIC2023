using UnityEngine;
using TMPro;
using System;
using ScriptableBehaviour;

namespace GhostNirvana.UI {

[RequireComponent(typeof(TMP_Text))]
public class LimitedResourceTextDisplay : MonoBehaviour {
    TMP_Text textObject;

    [SerializeField, Tooltip("For String.Format. {0} is the value of monitoredValue, {1} is its limiter.")] string format;
    [SerializeField] LinearLimiterFloat monitoredValue;

    void Awake() {
        textObject = GetComponent<TMP_Text>();
    }

    void LateUpdate() {
        UpdateDisplay();
    }

    public void UpdateDisplay() {
        textObject.text = String.Format(format, monitoredValue.Value, monitoredValue.Limiter);
    }
}

}
