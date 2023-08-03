using UnityEngine;
using TMPro;
using Base;
using DG.Tweening;
using NaughtyAttributes;
using Utils;
using System;
using System.Collections;

namespace CombatSystem.UI {

[RequireComponent(typeof(RectTransform))]
public class DamageNumber : Entity {
    TMP_Text text;
    CanvasGroup canvasGroup;
    RectTransform rectTransform;
    new Camera camera;
    Canvas canvas;
    RectTransform canvasRectTransform;

    [SerializeField] Vector3 worldSpaceOffset;
    [SerializeField] Color color;
    [SerializeField] float verticalMovementDuringAnimation;
    [SerializeField] float randomizedPositionRadius;
    [SerializeField, MinMaxSlider(0, 1)] Vector2 appearDurationRange;
    [SerializeField] Ease appearSizeEase;
    [SerializeField] Ease appearMovementEase;
    [SerializeField, MinMaxSlider(0, 1)] Vector2 disappearDurationRange;
    [SerializeField] Ease disappearSizeEase;
    [SerializeField] Ease disappearMovementEase;

    Vector3 textScale;

#nullable enable
    Sequence? animSequence;
#nullable disable

    void Awake() {
        text = GetComponentInChildren<TMP_Text>();
        canvasGroup = GetComponent<CanvasGroup>();

        textScale = text.transform.localScale;
        rectTransform = GetComponent<RectTransform>();
        camera = Camera.main;
        canvas = GetComponentInParent<Canvas>();
        canvasRectTransform = canvas.GetComponent<RectTransform>();
    }

    public void Initialize(float damage, Vector3 worldPosition) {
        text.color = color;
        text.text = String.Format("{0}", (int) damage);

        Vector2 anchorPosition = Calc.AnchorPositionFromWorld(
            camera, canvasRectTransform, worldPosition + worldSpaceOffset) + UnityEngine.Random.insideUnitCircle * randomizedPositionRadius;

        rectTransform.anchoredPosition = anchorPosition;

        canvasGroup.alpha = 1;
        text.transform.localScale = Vector3.zero;

        animSequence = DOTween.Sequence();

        float appearDuration = Mathx.RandomRange(appearDurationRange.x, appearDurationRange.y);
        float disappearDuration = Mathx.RandomRange(disappearDurationRange.x, disappearDurationRange.y);

        animSequence.Append(text.rectTransform.DOScale(textScale, appearDuration).SetEase(appearSizeEase));
        animSequence.Join(text.rectTransform.DOAnchorPosY(verticalMovementDuringAnimation, appearDuration).SetEase(appearMovementEase));

        animSequence.Append(text.rectTransform.DOScale(textScale / 2, disappearDuration).SetEase(disappearSizeEase));
        animSequence.Join(text.rectTransform.DOAnchorPosY(0, disappearDuration).SetEase(disappearMovementEase));

        animSequence.OnComplete(() => Dispose());
        animSequence.OnKill(() => Dispose());
    }

    void OnDisable() {
        // this should never be alive when this happen
        animSequence?.Kill();
    }

    protected override IEnumerator IDispose() {
        yield return null;
        gameObject.SetActive(false); // we don't destroy because the manager is managing this pool
    }
}

}
