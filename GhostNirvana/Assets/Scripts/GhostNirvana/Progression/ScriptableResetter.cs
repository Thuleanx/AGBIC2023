using UnityEngine;
using ScriptableBehaviour;
using NaughtyAttributes;

namespace GhostNirvana {

public class ScriptableResetter : MonoBehaviour {
    [SerializeField] Bank bank;
    [SerializeField, Expandable] StatsList stats;

    void Awake() => ResetAllScriptableVariables();

    void ResetAllScriptableVariables() {
        bank.Withraw(bank.Balance);
        foreach (LinearInt lint in stats.AllInts) {
            lint.AdditiveScale = 0;
            lint.MultiplicativeScale = 1;
            lint.Recompute();
            lint.OnAfterDeserialize();
        }
        foreach (LinearFloat lfloat in stats.AllFloats) {
            lfloat.AdditiveScale = 0;
            lfloat.MultiplicativeScale = 1;
            lfloat.Recompute();
            lfloat.OnAfterDeserialize();
        }
    }
}

}
