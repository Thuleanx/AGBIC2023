using UnityEngine;
using UnityEngine.UI;

namespace GhostNirvana {

[RequireComponent(typeof(Button))]
public class UpgradeOption : MonoBehaviour {
    [SerializeField] public Buff Buff {get; private set; }
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
    }

    void OnSelect() {
        Buff?.Apply();
        upgradeSystem?.EndLevelUpSequence();
    }

    public void EventTriggerOnly_OnHoverEnter() => OnHoverEnter?.Invoke(this);
    public void EventTriggerOnly_OnHoverExit() => OnHoverExit?.Invoke(this);
}

}
