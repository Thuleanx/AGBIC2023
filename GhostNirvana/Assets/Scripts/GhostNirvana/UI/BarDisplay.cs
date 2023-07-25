using UnityEngine;
using UnityEngine.UI;
using ScriptableBehaviour;

namespace GhostNirvana.UI {

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Slider))]
public class BarDisplay : MonoBehaviour {
    protected RectTransform rectTranform;
    protected Slider slider;

    [SerializeField] protected LinearLimiterFloat variable;

    void Awake() {
        rectTranform = GetComponent<RectTransform>();
        slider = GetComponent<Slider>();
    }

    protected virtual void LateUpdate() {
        // TODO: Optimize by having bar change on callback
        slider.value = variable.Value / variable.Limiter;
    }
}

}
