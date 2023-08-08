using System;
using UnityEngine;
using TMPro;
using NaughtyAttributes;

namespace GhostNirvana.Upgrade {

public class UpgradeMoneyDisplay : MonoBehaviour {
    const float CENTS_IN_DOLLAR = 100.0f;
    [SerializeField, Required] RectTransform upgradeOptionsParent;
    [SerializeField] Bank bank;

    [SerializeField] TMP_Text payDescription;
    [SerializeField] TMP_Text balanceMinusPriceDisplay;
    [SerializeField] TMP_Text priceDisplay;
    [ResizableTextArea, SerializeField] string payDescriptionFormatting;
    [ResizableTextArea, SerializeField] string balanceMinusPriceDisplayFormatting;
    [ResizableTextArea, SerializeField] string priceDisplayFormatting;

    void OnEnable() {
        foreach (UpgradeOption option in upgradeOptionsParent.GetComponentsInChildren<UpgradeOption>()) {
            option.OnHoverEnter += OnHover;
            option.OnHoverExit += OnHoverExit;
        }
        payDescription.gameObject.SetActive(true);
    }

    void OnDisable() {
        foreach (UpgradeOption option in upgradeOptionsParent.GetComponentsInChildren<UpgradeOption>()) {
            option.OnHoverEnter -= OnHover;
            option.OnHoverExit -= OnHoverExit;
        }
    }

    void OnHover(UpgradeOption option) {
        payDescription.gameObject.SetActive(false);
        SetUpgradeOption(option);
    }

    void OnHoverExit(UpgradeOption option) {
        // Here we assume it's impossible to hover two options at once
        payDescription.gameObject.SetActive(true);
        SetUpgradeOption(null);
    }

    void SetUpgradeOption(UpgradeOption option) {
        int price = (option ? option.Buff.Cost : 0);
        int balanceMinusPrice = bank.Value - price;

        balanceMinusPriceDisplay.text = String.Format(
            balanceMinusPriceDisplayFormatting,
            balanceMinusPrice / CENTS_IN_DOLLAR
        );

        bool shouldDisplayPrice = option;

        priceDisplay.gameObject.SetActive(shouldDisplayPrice);

        if (!shouldDisplayPrice) return;

        priceDisplay.text = String.Format(
            priceDisplayFormatting,
            price / CENTS_IN_DOLLAR
        );
    }

    public void SetPaymentDescription(int collectionCount, float paymentAmount) {
        payDescription.text = String.Format(
            payDescriptionFormatting,
            collectionCount, paymentAmount / CENTS_IN_DOLLAR
        );
    }
}

}
