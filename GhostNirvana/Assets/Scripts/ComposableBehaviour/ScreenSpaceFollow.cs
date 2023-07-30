using UnityEngine;
using NaughtyAttributes;
using Utils;

namespace ComposableBehaviour {

[RequireComponent(typeof(RectTransform))]
public class ScreenSpaceFollow : MonoBehaviour {
    Canvas canvas;
    RectTransform canvasRectTransform;
    RectTransform rectTransform;
    new Camera camera;

    [ReadOnly]
    public Transform Target;
    [SerializeField] Vector3 offset;

    void Awake() {
        canvas = GetComponentInParent<Canvas>();
        canvasRectTransform = canvas.GetComponent<RectTransform>();

        rectTransform = GetComponent<RectTransform>();
        camera = Camera.main;
    }

    void LateUpdate() {
        if (!Target) return;

        rectTransform.anchoredPosition = Calc.AnchorPositionFromWorld(
            camera: camera,
            canvasRectTransform: canvasRectTransform,
            worldPoint: Target.transform.position + offset
        );
    }
}

}
