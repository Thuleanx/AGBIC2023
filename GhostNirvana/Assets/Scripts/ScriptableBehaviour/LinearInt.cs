using UnityEngine;
using NaughtyAttributes;

namespace ScriptableBehaviour {

[CreateAssetMenu(fileName = "Data",
                 menuName = "~/Stats/LinearInt", order = 1)]
public class LinearInt : ScriptableInt, ISerializationCallbackReceiver {
    public int BaseValue = 0;
    [ReadOnly]
    public int AdditiveScale;
    [ReadOnly]
    public float MultiplicativeScale;

    public void OnAfterDeserialize() {
        AdditiveScale = 0;
        MultiplicativeScale = 1;
        Recompute();
    }

    public void OnBeforeSerialize() {
    }

    /// Need to be called every time either additive or multiplicative scale is changed.
    public void Recompute() {
        Value = (int) ((BaseValue + AdditiveScale) * MultiplicativeScale);
    }
}

}
