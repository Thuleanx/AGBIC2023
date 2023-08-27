using UnityEngine;
using DG.Tweening;
using NaughtyAttributes;

namespace UI {
	public class Scaler : MonoBehaviour {
        [SerializeField] float animationDelay;
        [SerializeField] float animationDuration;
        [SerializeField] Ease animationEase;
        [SerializeField] Vector3 target;
        [SerializeField] bool overrideStarting;
        [SerializeField, ShowIf("overrideStarting")] Vector3 targetOnAwake;

        [SerializeField] AutoAnimation startingAnimation;
        bool isExecuting;

        enum AutoAnimation {
            None,
            Scale
        };


        protected void Awake() {
            if (!overrideStarting)
                targetOnAwake = transform.localScale;
        }

        protected void OnEnable() {
            if (startingAnimation == AutoAnimation.Scale)
                Execute();
        }

        public (bool, YieldInstruction) Execute() {
            if (isExecuting) return (false, null);
            isExecuting = true;

            transform.localScale = targetOnAwake;
            Tween executionTween = transform.DOScale(
                target, animationDuration
            );
            executionTween.SetUpdate(isIndependentUpdate: true);
            executionTween.SetDelay(animationDelay);
            executionTween.SetEase(animationEase);
            executionTween.OnComplete( () => {
                isExecuting = false;
                transform.localScale = target;
            });
            executionTween.OnKill( () => {
                isExecuting = false;
            });

            return (true, executionTween.WaitForCompletion());
        }
	}
}
