using UnityEngine;
using UnityEngine.UI;

namespace GhostNirvana.UI {

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Slider))]
public class HealthBarDisplay : BarDisplay {
    [SerializeField] float hpWidthScale;
    [SerializeField] float maxWidth;

    protected override void LateUpdate() {
        // TODO: Optimize by having bar change on callback
        rectTranform.sizeDelta = new Vector2(
            Mathf.Clamp(hpWidthScale * variable.Limiter, 0, maxWidth),
            rectTranform.sizeDelta.y
        );
        base.LateUpdate();
    }
}

}
