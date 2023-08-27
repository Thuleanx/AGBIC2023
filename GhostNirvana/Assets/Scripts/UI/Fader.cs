using UnityEngine;
using DG.Tweening;
using NaughtyAttributes;

namespace UI {
	public class Fader : MonoBehaviour {
		[SerializeField] float fadeOutDuration;
		[SerializeField] float fadeInDuration;
		[SerializeField] CanvasGroup canvasGroup;

        enum State {
            FadingIn,
            Black,
            FadingOut,
            Transparent
        };

        [SerializeField] bool autoFadeIn;
        [SerializeField, ReadOnly] State currentState;

        void Awake() {
            if (autoFadeIn) {
                currentState = State.Black;
                canvasGroup.alpha = 1;
                FadeOut();
            } else {
                currentState = State.Transparent;
            }
        }

        void EnableCanvasGroup() {
            canvasGroup.alpha = 1;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        void DisableCanvasGroup() {
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        public (bool, YieldInstruction) FadeIn() {
            if (currentState != State.Transparent) return (false, null);
            currentState = State.FadingIn;
            canvasGroup.alpha = 0;
            Tween fadingTween = canvasGroup.DOFade(1, fadeInDuration);
            fadingTween.SetUpdate(isIndependentUpdate: true);
            fadingTween.OnComplete(() => {
                currentState = State.Black;
                EnableCanvasGroup();
            });
            return (true, fadingTween.WaitForCompletion());
        }

        public (bool, YieldInstruction) FadeOut() {
            if (currentState != State.Black) return (false, null);
            currentState = State.FadingOut;
            canvasGroup.alpha = 1;
            Tween fadingTween = canvasGroup.DOFade(0, fadeOutDuration);
            fadingTween.SetUpdate(isIndependentUpdate: true);
            fadingTween.OnComplete(() => {
                currentState = State.Transparent;
                DisableCanvasGroup();
            });
            return (true, fadingTween.WaitForCompletion());
        }
	}
}
