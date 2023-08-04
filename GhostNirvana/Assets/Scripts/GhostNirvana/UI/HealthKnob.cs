using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using NaughtyAttributes;
using DG.Tweening;

namespace GhostNirvana.UI {
    [RequireComponent(typeof(Image))]
    [RequireComponent(typeof(RectTransform))]
    public class HealthKnob : MonoBehaviour {
        public bool Filled {get; private set; }
        public bool LowActive {get; private set; }

        [field:SerializeField] public Image Image {get; private set;}
        public RectTransform rectTransform {get; private set; }
        [Space, SerializeField] UnityEvent OnFill;
        [Space, SerializeField] UnityEvent OnEmpty;

        [SerializeField, Required] Sprite filledSprite;
        [SerializeField, Required] Sprite emptySprite;

        [Header("Shake on Health Gain")]
        [SerializeField, Range(0,1)] float shakeDurationGained = 0.2f;
        [SerializeField, Range(0,100)] float shakeStrengthGained = 25;
        [SerializeField, Range(0,100)] int shakeFrequencyGained = 30;

        [Header("Shake on Health Lost")]
        [SerializeField, Range(0,1)] float shakeDurationLost = 0.2f;
        [SerializeField, Range(0,100)] float shakeStrengthLost = 25;
        [SerializeField, Range(0,300)] int shakeFrequencyLost = 30;

        [Header("Low health")]
        [SerializeField] UnityEvent onLowHealth;
        [SerializeField] UnityEvent onNotLowHealth;
        [SerializeField, Range(0,1)] float lowHealthLoopDuration;
        [SerializeField, Range(0,1)] float shakeDurationLow = 0.2f;
        [SerializeField, Range(0,100)] float shakeStrengthLow = 25;
        [SerializeField, Range(0,300)] int shakeFrequencyLow = 30;

        Sequence lowSequence;
        Vector2 originalAnchorPos;

        void Awake() {
            if (!Image) Image = GetComponent<Image>();
            rectTransform = GetComponent<RectTransform>();
        }

        void OnEnable() { 
            Filled = true;
        }

        void onFill() {
            lowSequence?.Kill();
            OnFill?.Invoke();
            Image.sprite = filledSprite;
        }
        void onEmpty() {
            lowSequence?.Kill();
            OnEmpty?.Invoke();
            Image.sprite = emptySprite;
            rectTransform.DOShakeAnchorPos(shakeDurationGained, shakeStrengthGained, shakeFrequencyGained);
            rectTransform.DOShakeRotation(shakeDurationLost, shakeStrengthLost, shakeFrequencyLost);
        }

        public void SetFilled(bool filled) {
            if (Filled ^ filled) {
                if (!Filled) onFill();
                else onEmpty();
                Filled = filled;
            }
        }
        public void SetLow(bool low) {
            if (low ^ LowActive) {
                if (low) {
                    onLowHealth?.Invoke();
                    lowSequence?.Kill();
                    originalAnchorPos = rectTransform.anchoredPosition;
                    lowSequence = DOTween.Sequence();
                    lowSequence.Append(rectTransform.DOShakeAnchorPos(shakeDurationLow, shakeStrengthLow, shakeFrequencyLow));
                    lowSequence.AppendInterval(lowHealthLoopDuration - shakeDurationLow);
                    lowSequence.SetLoops(-1).OnKill(
                            () => {
                            rectTransform.anchoredPosition = originalAnchorPos;
                            }
                            );
                } else {
                    rectTransform.anchoredPosition = originalAnchorPos;
                    lowSequence?.Kill();
                    onNotLowHealth?.Invoke();
                }
                LowActive = low;
            }
        }
    }
}
