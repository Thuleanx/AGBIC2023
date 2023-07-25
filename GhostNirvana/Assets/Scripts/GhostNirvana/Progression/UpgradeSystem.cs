using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using ScriptableBehaviour;

namespace GhostNirvana.Upgrade {

public class UpgradeSystem : MonoBehaviour {
    [SerializeField] LinearLimiterFloat experience;
    [SerializeField] LinearFloat applianceCollectionAmount;
    [SerializeField] Bank bank;
    [SerializeField, Required] RectTransform levelUpOptionPanel;
    bool levelUpSequenceRunning;
    List<UpgradeOption> upgradeOptions = new List<UpgradeOption>();

    [SerializeField, ReorderableList]
    List<Buff> buffOptions = new List<Buff>();

    ApplianceCollector collector;

    void Awake() {
        collector = GetComponentInChildren<ApplianceCollector>();

        upgradeOptions.AddRange(GetComponentsInChildren<UpgradeOption>());
        levelUpOptionPanel.gameObject.SetActive(false);
    }

    void Update() {
        bool shouldLevelUp = !levelUpSequenceRunning && experience.Value >= experience.Limiter;
        if (shouldLevelUp) StartLevelUpSequence();
    }

    void StartLevelUpSequence() {
        levelUpOptionPanel.gameObject.SetActive(true);
        levelUpSequenceRunning = true;

        foreach (UpgradeOption upgradeOption in upgradeOptions) {
            Buff randomBuff = buffOptions[Random.Range(0, buffOptions.Count)];
            upgradeOption.Initialize(buff: randomBuff);
        }

        experience.Value -= experience.Limiter;
        experience.MultiplicativeScale *= 1.2f;
        experience.Recompute();

        collector.Collect((int) applianceCollectionAmount.Value);

        Time.timeScale = 0;
    }

    public void EndLevelUpSequence() {
        levelUpSequenceRunning = false;
        levelUpOptionPanel.gameObject.SetActive(false);
        Time.timeScale = 1;
    }
}

}
