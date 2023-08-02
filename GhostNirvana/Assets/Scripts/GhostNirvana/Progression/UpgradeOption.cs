using UnityEngine;
using UnityEngine.UI;

namespace GhostNirvana.Upgrade {

[RequireComponent(typeof(Button))]
public class UpgradeOption : MonoBehaviour {
    public Buff Buff {get; private set; }
    [field:SerializeField] public Bank PlayerBank {get; private set; }
    UpgradeSystem upgradeSystem;

    public delegate void HoverHandler(UpgradeOption option);
    public delegate void HoverExitHandler(UpgradeOption option);

    public event HoverHandler OnHoverEnter;
    public event HoverExitHandler OnHoverExit;

#region Components
    Button button;
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
    }

    void OnSelect() {
        Buff?.Apply();
        upgradeSystem?.EndLevelUpSequence(this.Buff);
    }

    public bool CanPurchase => PlayerBank && Buff && PlayerBank.Value >= Buff.cost;
    public void EventTriggerOnly_OnHoverEnter() => OnHoverEnter?.Invoke(this);
    public void EventTriggerOnly_OnHoverExit() => OnHoverExit?.Invoke(this);
}

}
