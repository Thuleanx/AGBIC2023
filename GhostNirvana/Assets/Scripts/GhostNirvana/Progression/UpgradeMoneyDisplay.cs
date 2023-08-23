using System;
using UnityEngine;
using TMPro;
using NaughtyAttributes;
using System.Collections.Generic;
using Utils;

namespace GhostNirvana.Upgrade {

public class UpgradeMoneyDisplay : MonoBehaviour {
    const float CENTS_IN_DOLLAR = 100.0f;
    [SerializeField, Required] RectTransform upgradeOptionsParent;
    [SerializeField] Bank bank;

    [SerializeField, ColorUsage(true, true)] Color textColorForGains;
    [SerializeField, ColorUsage(true, true)] Color textColorForLoss;
    [SerializeField] TMP_Text payDescription;
    [SerializeField] TMP_Text balanceMinusPriceDisplay;
    [SerializeField] TMP_Text priceDisplay;
    [ResizableTextArea, SerializeField] string payDescriptionFormatting;
    [ResizableTextArea, SerializeField] string balanceMinusPriceDisplayFormatting;
    [ResizableTextArea, SerializeField] string priceDisplayFormatting;
    [SerializeField] float exitBufferTime = 1;
    bool shouldHoverExit;
    float hoverExitTime;

    [SerializeField, ResizableTextArea, ReorderableList] List<string> tipTexts;

    void OnEnable() {
        foreach (UpgradeOption option in upgradeOptionsParent.GetComponentsInChildren<UpgradeOption>()) {
            option.OnHoverEnter += OnHover;
            option.OnHoverExit += OnHoverExit;
        }
        payDescription.gameObject.SetActive(true);
        balanceMinusPriceDisplay.text = String.Format(
            balanceMinusPriceDisplayFormatting,
            bank.Value / CENTS_IN_DOLLAR
        );
        priceDisplay.gameObject.SetActive(false);
    }

    void OnDisable() {
        foreach (UpgradeOption option in upgradeOptionsParent.GetComponentsInChildren<UpgradeOption>()) {
            option.OnHoverEnter -= OnHover;
            option.OnHoverExit -= OnHoverExit;
        }
    }

    void OnHover(UpgradeOption option) {
        shouldHoverExit = false;
        payDescription.gameObject.SetActive(false);
        SetUpgradeOption(option);
    }

    void OnHoverExit(UpgradeOption option) {
        hoverExitTime = exitBufferTime + Time.unscaledTime;
        shouldHoverExit = true;
    }

    void DisplayStatusDescription() {
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
        balanceMinusPriceDisplay.color = balanceMinusPrice >= 0 ? textColorForGains : textColorForLoss;

        bool shouldDisplayPrice = option;

        priceDisplay.gameObject.SetActive(shouldDisplayPrice);

        if (!shouldDisplayPrice) return;

        priceDisplay.text = String.Format(
            priceDisplayFormatting,
            price / CENTS_IN_DOLLAR
        );
        priceDisplay.color = textColorForLoss;
    }

    public void SetPaymentDescription(string rank, int collectionCount, float paymentAmount) {
        string tip = "";
        if (tipTexts.Count > 0)
            tip = tipTexts[Mathx.RandomRange(0, tipTexts.Count)];
        payDescription.text = String.Format(
            payDescriptionFormatting,
            rank, collectionCount, paymentAmount / CENTS_IN_DOLLAR,
            tip
        );
    }

    protected void LateUpdate() {
        if (shouldHoverExit && Time.unscaledTime > hoverExitTime) {
            DisplayStatusDescription();
            shouldHoverExit = false;
        }
    }
}

}
