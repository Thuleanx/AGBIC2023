using UnityEngine;
using NaughtyAttributes;

namespace ScriptableBehaviour {

[CreateAssetMenu(fileName = "Data",
                 menuName = "~/Stats/LinearLimitedFloat", order = 1)]
public class LinearLimiterFloat : ScriptableFloat, ISerializationCallbackReceiver {
    public float BaseMaxValue = 0;
    /* [System.NonSerialized] */
    [ReadOnly]
    public float AdditiveScale;

    [ReadOnly]
    public float MultiplicativeScale;

    [ReadOnly]
    public float Limiter;

    public void OnAfterDeserialize() {
        AdditiveScale = 0;
        MultiplicativeScale = 1;
        Recompute();
        Value = 0;
    }

    public void OnBeforeSerialize() {
    }

    /// Need to be called every time either additive or multiplicative scale is changed.
    public void Recompute() {
        Limiter = (BaseMaxValue + AdditiveScale) * MultiplicativeScale;
    }

    public void CheckAndCorrectLimit() {
        Value = Mathf.Clamp(Value, 0, Limiter);
    }
}

}
