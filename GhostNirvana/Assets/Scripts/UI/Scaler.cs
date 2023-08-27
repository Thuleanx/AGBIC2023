using UnityEngine;
using DG.Tweening;

namespace UI {
    public class Scaler : SingleTween<Vector3> {
        protected override Vector3 Value { 
            get => transform.localScale; 
            set => transform.localScale = value;
        }

        protected override Tween GetTween(float duration) => transform.DOScale(target, duration);
    }
}
