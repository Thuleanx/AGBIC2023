using UnityEngine;
using NaughtyAttributes;

namespace ComposableBehaviour {

[RequireComponent(typeof(Collider))]
public class ColliderActiveDuration : MonoBehaviour {
    [SerializeField, MinMaxSlider(0, 10)] Vector2 activeDuration;
    new Collider collider;

    float timeElapsed = 0;

    void Awake() {
        collider = GetComponent<Collider>();
    }

    void OnEnable() {
        collider.enabled = activeDuration.x == 0;
        timeElapsed = 0;
    }

    void Update() {
        timeElapsed += Time.deltaTime;

        bool shouldEnableCollider = timeElapsed >= activeDuration.x && timeElapsed <= activeDuration.y;
        if (shouldEnableCollider ^ collider.enabled)
            collider.enabled = shouldEnableCollider;
    }
}

}

