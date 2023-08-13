using System;
using UnityEngine;
using TMPro;
using NaughtyAttributes;

namespace GhostNirvana.Upgrade {

public class UpgradeOptionDetails : MonoBehaviour {
    [SerializeField, Required] RectTransform upgradeOptionsParent;

    [SerializeField] TMP_Text title;
    [SerializeField] TMP_Text description;
    [SerializeField] TMP_Text price;

    void OnEnable() {
        foreach (UpgradeOption option in upgradeOptionsParent.GetComponentsInChildren<UpgradeOption>()) {
            option.OnHoverEnter += OnHover;
            option.OnHoverExit += OnHoverExit;
        }
        title.gameObject.SetActive(false);
        description.gameObject.SetActive(false);
        price.gameObject.SetActive(false);
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
        price.gameObject.SetActive(true);

        title.text = option.Buff.name;
        description.text = option.Buff.description;
		price.text = String.Format("{0:C2}", option.Buff.Cost / 100.0f);
    }

    void OnHoverExit(UpgradeOption option) {
        // Here we assume it's impossible to hover two options at once
        title.gameObject.SetActive(false);
        description.gameObject.SetActive(false);
        price.gameObject.SetActive(false);
    }
}

}
