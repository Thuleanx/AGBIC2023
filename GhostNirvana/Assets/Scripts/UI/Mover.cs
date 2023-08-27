using UnityEngine;
using DG.Tweening;

namespace UI {
    public class Mover : SingleTween<Vector2> {
        RectTransform rectTransform;
        Vector2 originalAnchor;

        protected override Vector2 Value {
            get => rectTransform.anchoredPosition;
            set => rectTransform.anchoredPosition = value + originalAnchor;
        }

        protected override void Awake() {
            rectTransform = GetComponent<RectTransform>();
            base.Awake();
        }

        protected override void OnEnable() {
            originalAnchor = rectTransform.anchoredPosition;
            base.OnEnable();
        }

        protected override Tween GetTween(float duration)
            => rectTransform.DOAnchorPos(target + originalAnchor, duration);
    }
}
