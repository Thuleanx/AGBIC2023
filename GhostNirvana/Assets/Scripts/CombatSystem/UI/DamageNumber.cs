using UnityEngine;
using TMPro;
using DG.Tweening;
using Optimization;

namespace CombatSystem.UI {

[RequireComponent(typeof(TMP_Text))]
public class DamageNumber : MonoBehaviour {
    TMP_Text text;

    [SerializeField] float randomPositionRadius;
    [SerializeField] Color color;

    public void Initialize(int number, Vector3 position) {
        text.color = color;
        text.text = number.ToString();

        Vector3 randomizedPosition = randomPositionRadius * Random.insideUnitSphere;
        text.rectTransform.position = position + randomizedPosition;
    }
}

}
