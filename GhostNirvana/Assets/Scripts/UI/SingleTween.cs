using UnityEngine;
using DG.Tweening;
using NaughtyAttributes;
using Utils;

namespace UI {
	public abstract class SingleTween<T> : MonoBehaviour {
        [SerializeField, MinMaxSlider(0, 5)] protected Vector2 animationDelay;
        [SerializeField, MinMaxSlider(0, 5)] protected Vector2 animationDuration;
        [SerializeField] protected Ease animationEase;
        [SerializeField] protected T target;
        [SerializeField] protected bool overrideStarting;
        [SerializeField, ShowIf("overrideStarting")] protected T targetOnAwake;

        [SerializeField] protected AutoAnimation startingAnimation;
        protected bool isExecuting;

        protected abstract T Value { get; set; }
        protected abstract Tween GetTween(float duration);

        public enum AutoAnimation {
            None,
            Animate
        };

        protected virtual void Awake() {
            if (!overrideStarting) targetOnAwake = Value;
        }

        protected virtual void OnEnable() {
            isExecuting = false;
            if (startingAnimation == AutoAnimation.Animate) Execute();
        }

        public void ExecuteAsEvent() {
            var (executed, _) = Execute();
        }

        public (bool, YieldInstruction) Execute() {
            if (isExecuting) return (false, null);
            isExecuting = true;

            Value = targetOnAwake;
            float actualDuration = Mathx.RandomRange(animationDuration);
            float actualDelay = Mathx.RandomRange(animationDelay);
            Tween executionTween = GetTween(actualDuration);
            executionTween.SetUpdate(isIndependentUpdate: true);
            executionTween.SetDelay(actualDelay);
            executionTween.SetEase(animationEase);
            executionTween.OnComplete( () => {
                isExecuting = false;
                Value = target;
            });
            executionTween.OnKill( () => {
                isExecuting = false;
            });
            executionTween.Play();
            return (true, executionTween.WaitForCompletion());
        }
	}
}
