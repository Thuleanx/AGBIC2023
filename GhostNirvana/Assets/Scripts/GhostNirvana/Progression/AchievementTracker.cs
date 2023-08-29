using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;
using TMPro;
using System;
using System.Collections.Generic;
using ScriptableBehaviour;

namespace GhostNirvana {

public class AchievementTracker : MonoBehaviour {
    [SerializeField] ScriptableFloat time;
    [SerializeField, Required] Bank bank;
    [SerializeField] MovableAgentRuntimeSet allEnemies;
    [SerializeField] MovableAgentRuntimeSet allAppliances;

    [SerializeField, ReadOnly] int enemiesKilled;
    [SerializeField, ReadOnly] int grossIncome;
    [SerializeField, ReadOnly] int profit;

    [SerializeField] Image buffIcon;

    [SerializeField]
    Upgrade.UpgradeSystem upgradeSystem;
    [SerializeField]
    ApplianceCollector applianceCollector;

    public int ApplianceCollected => applianceCollector.numCollected;
    public int Time => Mathf.RoundToInt(time.Value * 60);
    public int MoneyEarned => grossIncome;
    public int MoneyTakeHome => profit;
    public List<int> BuffsTaken {
        get {
            List<int> buffs = new List<int>();
            foreach (Buff buff in upgradeSystem.BuffsTakenInSequence)
                buffs.Add(buff.id);
            return buffs;
        }
    }

    void OnEnable() {
        enemiesKilled = 0;
        allEnemies.OnRemove += OnEnemyRemoved;
        bank.OnDeposit.AddListener(OnDeposit);
        bank.OnWithraw.AddListener(OnWithraw);
    }

    void OnDisable() {
        allEnemies.OnRemove -= OnEnemyRemoved;
        bank.OnWithraw.RemoveListener(OnWithraw);
        bank.OnDeposit.RemoveListener(OnDeposit);
    }

    void OnEnemyRemoved(MovableAgent agent) {
        if (agent is Ghosty) enemiesKilled++;
    }

    void OnDeposit(int money) {
        grossIncome += money;
        profit += money;
    }

    void OnWithraw(int money) => profit -= money;

    public void PopulateText(TMP_Text textbox) {
        int secondsInMinutes = 60;
        int numMinutes = (int) (time.Value);
        int numSeconds = (int) (time.Value * secondsInMinutes) % secondsInMinutes;

        string timeDisplay = String.Format("{0,2:D2}:{1,2:D2}", numMinutes, numSeconds);

        textbox.text = String.Format(
            textbox.text,
            timeDisplay,
            enemiesKilled,
            applianceCollector.numCollected,
            grossIncome / 100.0f,
            profit / 100.0f,
            upgradeSystem.GetRank()
        );
    }

    public void PopulateChildrenWithBuffIcons(RectTransform parent) {
        foreach (Buff buff in upgradeSystem.BuffsTakenInSequence) {
            Image image = Instantiate(buffIcon, parent);
            image.sprite = buff.icon;
        }
    }
}

}
