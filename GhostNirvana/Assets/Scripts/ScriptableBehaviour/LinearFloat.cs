using UnityEngine;
using NaughtyAttributes;
using System;

namespace ScriptableBehaviour {

[CreateAssetMenu(fileName = "Data",
                 menuName = "~/Stats/LinearFloat", order = 1)]
public class LinearFloat : ScriptableFloat, ISerializationCallbackReceiver, ILinearlyScalable<float> {
    public float BaseValue = 0;
    [NonSerialized]
    public float AdditiveScale;
    [NonSerialized]
    public float MultiplicativeScale;

    float ILinearlyScalable<float>.AdditiveScale { get => AdditiveScale; set => AdditiveScale = value; }
    float ILinearlyScalable<float>.MultiplicativeScale { get => MultiplicativeScale; set => MultiplicativeScale = value; }

    public virtual void OnAfterDeserialize() {
        AdditiveScale = 0;
        MultiplicativeScale = 1;
        Recompute();
    }

    public virtual void OnBeforeSerialize() {
    }

    /// Need to be called every time either additive or multiplicative scale is changed.
    public virtual void Recompute() {
        Value = (BaseValue + AdditiveScale) * MultiplicativeScale;
    }
}

}
