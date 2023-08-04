using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using ScriptableBehaviour;


namespace GhostNirvana.UI {

[RequireComponent(typeof(RectTransform))]
public class HealthBarDisplay : MonoBehaviour {
    List<HealthKnob> healthKnotches = new List<HealthKnob>();
    [SerializeField] GameObject healthPrefab;
    [SerializeField] LinearLimiterFloat health;

    void Start() {

    }

    void Update() {
        UpdateNumberOfKnotches();
        UpdateFill();
    }

    void UpdateNumberOfKnotches() {
        int MaxHealth = Mathf.RoundToInt(health.Limiter);
        while (MaxHealth > healthKnotches.Count) {
            GameObject healthKnotch = Instantiate(healthPrefab, transform);
            healthKnotches.Add(healthKnotch.GetComponent<HealthKnob>());
        }
        while (MaxHealth < healthKnotches.Count) {
            int indexToRemove = healthKnotches.Count - 1;
            Destroy(healthKnotches[indexToRemove].gameObject);
            healthKnotches.RemoveAt(indexToRemove);
        }
    }

    void UpdateFill() {
        int Health = Mathf.RoundToInt(health.Value);
        for (int i = 0; i < health.Limiter; i++) 
            healthKnotches[i].SetFilled(Health > i);
        healthKnotches[0].SetLow(Health == 1);
    }
}

}
