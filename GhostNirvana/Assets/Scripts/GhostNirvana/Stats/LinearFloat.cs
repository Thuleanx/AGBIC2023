using UnityEngine;

namespace GhostNirvana {

[CreateAssetMenu(fileName = "Data",
                 menuName = "~/Stats/LinearFloat", order = 1)]
public class LinearFloat : ScriptableFloat, ISerializationCallbackReceiver {
    public float BaseValue = 0;
    [System.NonSerialized] public float AdditiveScale;
    [System.NonSerialized] public float MultiplicativeScale;

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
