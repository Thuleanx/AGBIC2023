using UnityEngine;
using NaughtyAttributes;

namespace ScriptableBehaviour {

[CreateAssetMenu(fileName = "Data",
                 menuName = "~/Stats/LinearFloat", order = 1)]
public class LinearFloat : ScriptableFloat, ISerializationCallbackReceiver {
    public float BaseValue = 0;
    [ReadOnly]
    public float AdditiveScale;
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
        Value = (BaseValue + AdditiveScale) * MultiplicativeScale;
    }
}

}
