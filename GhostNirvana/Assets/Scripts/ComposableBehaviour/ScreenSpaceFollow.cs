using UnityEngine;
using NaughtyAttributes;

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

        Vector2 ViewportPosition=Camera.main.WorldToViewportPoint(Target.transform.position + offset);
        Vector2 WorldObject_ScreenPosition=new Vector2(
            ((ViewportPosition.x*canvasRectTransform.sizeDelta.x)-(canvasRectTransform.sizeDelta.x*0.5f)),
            ((ViewportPosition.y*canvasRectTransform.sizeDelta.y)-(canvasRectTransform.sizeDelta.y*0.5f)));

        // Set
        rectTransform.anchoredPosition = WorldObject_ScreenPosition;
    }
}

}
