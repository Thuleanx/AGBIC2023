using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace GhostNirvana.Upgrade {

[RequireComponent(typeof(Button))]
public class UpgradeOption : MonoBehaviour {
    public Buff Buff {get; private set; }
    [field:SerializeField] public Bank PlayerBank {get; private set; }

    [Header("Animation")]
    [SerializeField] float expandTime;
    [SerializeField] float retractTime;
    [SerializeField] float expandScale;
    UpgradeSystem upgradeSystem;

    public delegate void HoverHandler(UpgradeOption option);
    public delegate void HoverExitHandler(UpgradeOption option);

    public event HoverHandler OnHoverEnter;
    public event HoverExitHandler OnHoverExit;

    Tween currentTween;

#region Components
    Button button;
    public Image Image;
#endregion

    void Awake() {
        button = GetComponent<Button>();
        upgradeSystem = GetComponentInParent<UpgradeSystem>();
    }

    void OnEnable() => button.onClick.AddListener(OnSelect);
    void OnDisable() => button.onClick.RemoveListener(OnSelect);

    public void Initialize(Buff buff) {
        this.Buff = buff;
        // do some other stuff here
        button.interactable = CanPurchase;
        Image.sprite = buff.icon;
    }

    void OnSelect() {
        Buff?.Apply();
        upgradeSystem?.EndLevelUpSequence(this.Buff);
    }

    public bool CanPurchase => PlayerBank && Buff && PlayerBank.Value >= Buff.Cost;
    public void EventTriggerOnly_OnHoverEnter() {
        OnHoverEnter?.Invoke(this);
        currentTween?.Kill();
        currentTween = transform.DOScale(expandScale * Vector3.one, duration: expandTime);
        currentTween.SetUpdate(isIndependentUpdate: true);
        currentTween.Play();
    }

    public void EventTriggerOnly_OnHoverExit() {
        OnHoverExit?.Invoke(this);
        currentTween?.Kill();
        currentTween = transform.DOScale(Vector3.one, duration: retractTime);
        currentTween.SetUpdate(isIndependentUpdate: true);
        currentTween.Play();
    }
}

}
