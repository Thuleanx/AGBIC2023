using UnityEngine;
using NaughtyAttributes;

namespace ScriptableBehaviour {

[CreateAssetMenu(fileName = "Data",
                 menuName = "~/Stats/LinearLimiterInt", order = 1)]
public class LinearLimiterInt : LinearInt, ISerializationCallbackReceiver, ILimited<int> {
    [ReadOnly]
    public int Limiter;

    int ILimited<int>.Value { get => Value; set => Value = value; }

    public override void OnAfterDeserialize() {
        base.OnAfterDeserialize();
        Value = 0;
    }

    public override void OnBeforeSerialize() {}

    /// Need to be called every time either additive or multiplicative scale is changed.
    public override void Recompute() => Limiter = (int) ((BaseValue + AdditiveScale) * MultiplicativeScale);
    public void CheckAndCorrectLimit() => Value = Mathf.Clamp(Value, 0, Limiter);
}

}
