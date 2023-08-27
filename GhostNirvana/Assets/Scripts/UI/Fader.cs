using UnityEngine;
using DG.Tweening;
using NaughtyAttributes;

namespace UI {
	public class Fader : MonoBehaviour {
        [SerializeField] float fadeOutDelay;
		[SerializeField] float fadeOutDuration;
        [SerializeField] Ease fadeOutEase = Ease.Linear;
        [SerializeField] float fadeInDelay;
		[SerializeField] float fadeInDuration;
        [SerializeField] Ease fadeInEase = Ease.Linear;
		[SerializeField] CanvasGroup canvasGroup;

        enum State {
            FadingIn,
            Black,
            FadingOut,
            Transparent
        };

        enum AutoAnimation {
            None,
            FadeIn,
            FadeOut
        };

        [SerializeField] AutoAnimation startingAnimation;
        [SerializeField, ReadOnly] State currentState;

        float alphaAtAwake;

        void Awake() {
            alphaAtAwake = canvasGroup.alpha;
        }

        void OnEnable() {
            if (startingAnimation == AutoAnimation.FadeOut) {
                currentState = State.Black;
                EnableCanvasGroup();
                FadeOut();
            } else if (startingAnimation == AutoAnimation.FadeIn) {
                currentState = State.Transparent;
                DisableCanvasGroup();
                FadeIn();
            } else {
                currentState = alphaAtAwake == 0 ? State.Transparent : State.Black;
                if (currentState == State.Transparent)
                    DisableCanvasGroup();
                else EnableCanvasGroup();
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

        public void FadeInAsEvent() => FadeIn();
        public void FadeOutAsEvent() => FadeOut();

        public (bool, YieldInstruction) FadeIn() {
            if (currentState != State.Transparent) return (false, null);
            currentState = State.FadingIn;
            canvasGroup.alpha = 0;
            Tween fadingTween = canvasGroup.DOFade(1, fadeInDuration);
            fadingTween.SetDelay(fadeInDelay);
            fadingTween.SetUpdate(isIndependentUpdate: true);
            fadingTween.SetEase(fadeInEase);
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
            fadingTween.SetDelay(fadeOutDelay);
            fadingTween.SetEase(fadeOutEase);
            fadingTween.OnComplete(() => {
                currentState = State.Transparent;
                DisableCanvasGroup();
            });
            return (true, fadingTween.WaitForCompletion());
        }
	}
}
