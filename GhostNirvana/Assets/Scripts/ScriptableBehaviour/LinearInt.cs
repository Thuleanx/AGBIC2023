using UnityEngine;
using NaughtyAttributes;

namespace ScriptableBehaviour {

[CreateAssetMenu(fileName = "Data",
                 menuName = "~/Stats/LinearInt", order = 1)]
public class LinearInt : ScriptableInt, ISerializationCallbackReceiver, ILinearlyScalable<int> {
    public int BaseValue = 0;
    [ReadOnly]
    public float AdditiveScale;
    [ReadOnly]
    public float MultiplicativeScale;

    float ILinearlyScalable<int>.AdditiveScale { get => AdditiveScale; set => AdditiveScale = value; }
    float ILinearlyScalable<int>.MultiplicativeScale { get => MultiplicativeScale; set => MultiplicativeScale = value; }

    public virtual void OnAfterDeserialize() {
        AdditiveScale = 0;
        MultiplicativeScale = 1;
        Recompute();
    }

    public virtual void OnBeforeSerialize() {
    }

    /// Need to be called every time either additive or multiplicative scale is changed.
    public virtual void Recompute() {
        Value = (int) ((BaseValue + AdditiveScale) * MultiplicativeScale);
    }
}

}
