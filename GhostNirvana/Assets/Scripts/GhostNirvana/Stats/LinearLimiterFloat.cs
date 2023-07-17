using UnityEngine;

namespace GhostNirvana {

[CreateAssetMenu(fileName = "Data",
                 menuName = "~/Stats/LinearLimitedFloat", order = 1)]
public class LinearLimiterFloat : ScriptableFloat, ISerializationCallbackReceiver {
    public float BaseMaxValue = 0;
    [System.NonSerialized] public float AdditiveScale;
    [System.NonSerialized] public float MultiplicativeScale;

    [System.NonSerialized] public float Limiter;

    public void OnAfterDeserialize() {
        AdditiveScale = 0;
        MultiplicativeScale = 1;
        Recompute();
        Value = Limiter;
    }

    public void OnBeforeSerialize() {
    }

    /// Need to be called every time either additive or multiplicative scale is changed.
    public void Recompute() {
        Limiter = (BaseMaxValue + AdditiveScale) * MultiplicativeScale;
        CheckAndCorrectLimit();
    }

    public void CheckAndCorrectLimit() {
        Value = Mathf.Clamp(Value, 0, Limiter);
    }
}

}
