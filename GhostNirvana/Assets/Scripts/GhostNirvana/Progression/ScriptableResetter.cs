using UnityEngine;
using ScriptableBehaviour;
using System.Collections.Generic;

namespace GhostNirvana {

public class ScriptableResetter : MonoBehaviour {
    [SerializeField] Bank bank;
    [SerializeField] List<LinearInt> intsToReset;
    [SerializeField] List<LinearFloat> floatsToReset;

    void OnEnable() => ResetAllScriptableVariables();

    void ResetAllScriptableVariables() {
        bank.Withraw(bank.Balance);
        foreach (LinearInt lint in intsToReset) {
            lint.AdditiveScale = 0;
            lint.MultiplicativeScale = 1;
            lint.Recompute();
        }
        foreach (LinearFloat lfloat in floatsToReset) {
            lfloat.AdditiveScale = 0;
            lfloat.MultiplicativeScale = 1;
            lfloat.Recompute();
        }
    }
}

}
