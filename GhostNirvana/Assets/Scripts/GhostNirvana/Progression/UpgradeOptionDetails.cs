using System;
using UnityEngine;
using TMPro;
using NaughtyAttributes;

namespace GhostNirvana.Upgrade {

public class UpgradeOptionDetails : MonoBehaviour {
    [SerializeField, Required] RectTransform upgradeOptionsParent;

    [SerializeField] TMP_Text title;
    [SerializeField] TMP_Text description;
    [SerializeField] TMP_Text payDescription;
    [ResizableTextArea, SerializeField] string payDescriptionFormatting;

    void OnEnable() {
        foreach (UpgradeOption option in upgradeOptionsParent.GetComponentsInChildren<UpgradeOption>()) {
            option.OnHoverEnter += OnHover;
            option.OnHoverExit += OnHoverExit;
        }
        title.gameObject.SetActive(false);
        description.gameObject.SetActive(false);
        payDescription.gameObject.SetActive(true);
    }

    void OnDisable() {
        foreach (UpgradeOption option in upgradeOptionsParent.GetComponentsInChildren<UpgradeOption>()) {
            option.OnHoverEnter -= OnHover;
            option.OnHoverExit -= OnHoverExit;
        }
    }

    void OnHover(UpgradeOption option) {
        title.gameObject.SetActive(true);
        description.gameObject.SetActive(true);
        payDescription.gameObject.SetActive(false);

        title.text = option.Buff.name;
        description.text = option.Buff.description;
    }

    void OnHoverExit(UpgradeOption option) {
        // Here we assume it's impossible to hover two options at once
        title.gameObject.SetActive(false);
        description.gameObject.SetActive(false);
        payDescription.gameObject.SetActive(true);
    }

    public void SetPaymentDescription(int collectionCount, float paymentAmount) {
        payDescription.text = String.Format(
            payDescriptionFormatting,
            collectionCount, ((float) paymentAmount) / 100
        );
    }
}

}
