using UnityEngine;
using DG.Tweening;
using NaughtyAttributes;
using Utils;
using System.Collections;

namespace UI {
	public abstract class DualTween<T> : MonoBehaviour {
        [SerializeField, MinMaxSlider(0, 5)] protected Vector2 animationDelay;
        [SerializeField, MinMaxSlider(0, 5)] protected Vector2 animationDuration;
        [SerializeField] protected Ease animationEase;
        [SerializeField] protected T target;
        [SerializeField] protected bool overrideStarting;
        [SerializeField, ShowIf("overrideStarting")] protected T targetOnAwake;

        [SerializeField] protected AutoAnimation startingAnimation;

        protected abstract T Value { get; set; }
        protected abstract Tween GetTweenToTarget(float duration);
        protected abstract Tween GetTweenToOriginal(float duration);

        protected Tween currentTween;

        public enum AutoAnimation {
            None,
            ToTarget,
            ToOriginal
        };

        public enum State {
            MovingToTarget,
            AtTarget,
            MovingToOriginal,
            AtOriginal
        };
        [SerializeField, ReadOnly] protected State currentState;

        protected virtual void Awake() {
            if (!overrideStarting) targetOnAwake = Value;
        }

        protected virtual void OnEnable() {
            currentState = State.AtOriginal;
            Value = targetOnAwake;

            if (startingAnimation == AutoAnimation.ToTarget) {
                Value = targetOnAwake;
                StartCoroutine(GoToTarget());
            } else if (startingAnimation == AutoAnimation.ToOriginal) {
                Value = target;
                StartCoroutine(GoToOriginal());
            }
        }

        public void Event_GoToTarget() => StartCoroutine(GoToTarget());
        public void Event_GoToOriginal() => StartCoroutine(GoToOriginal());

        public IEnumerator GoToTarget() {
            if (currentState == State.AtTarget) yield break;
            if (currentTween?.IsActive() ?? false)
                currentTween?.Kill();

            currentTween = null;
            currentState = State.MovingToTarget;
            float actualDuration = Mathx.RandomRange(animationDuration);
            float actualDelay = Mathx.RandomRange(animationDelay);
            Tween executionTween = GetTweenToTarget(actualDuration);
            executionTween.SetUpdate(isIndependentUpdate: true);
            executionTween.SetDelay(actualDelay);
            executionTween.SetEase(animationEase);
            executionTween.OnComplete( () => {
                currentState = State.AtTarget;
                Value = target;
            });
            executionTween.Play();
            currentTween = executionTween;
            while (executionTween?.IsActive() ?? executionTween?.IsPlaying() ?? false)
                yield return null;
        }

        public IEnumerator GoToOriginal() {
            if (currentState == State.AtOriginal) yield break;
            currentTween?.Kill();
            currentTween = null;

            currentState = State.MovingToOriginal;
            float actualDuration = Mathx.RandomRange(animationDuration);
            float actualDelay = Mathx.RandomRange(animationDelay);
            Tween executionTween = GetTweenToOriginal(actualDuration);
            executionTween.SetUpdate(isIndependentUpdate: true);
            executionTween.SetDelay(actualDelay);
            executionTween.SetEase(animationEase);
            executionTween.OnComplete( () => {
                currentState = State.AtOriginal;
                Value = targetOnAwake;
            });
            executionTween.Play();
            currentTween = executionTween;
            while (executionTween?.IsActive() ?? executionTween?.IsPlaying() ?? false)
                yield return null;
        }
	}
}
