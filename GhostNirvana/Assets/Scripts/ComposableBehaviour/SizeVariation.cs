using UnityEngine;
using NaughtyAttributes;
using Utils;

namespace ComposableBehaviour {

public class SizeVariation : MonoBehaviour {
    Vector3 originalSize;

    [SerializeField, MinMaxSlider(0, 3f)] Vector2 deviation = Vector2.one;

    void Awake() {
        originalSize = transform.localScale;
    }

    void OnEnable() {
        transform.localScale = originalSize * Mathx.RandomRange(deviation.x, deviation.y);
    }
}

}
