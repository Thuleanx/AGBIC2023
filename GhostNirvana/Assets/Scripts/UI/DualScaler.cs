using UnityEngine;
using DG.Tweening;

namespace UI {
    public class DualScaler : DualTween<Vector3> {
        protected override Vector3 Value { 
            get => transform.localScale; 
            set => transform.localScale = value;
        }

        protected override Tween GetTweenToTarget(float duration)
            => transform.DOScale(target, duration);

        protected override Tween GetTweenToOriginal(float duration)
            => transform.DOScale(targetOnAwake, duration);
    }
}
