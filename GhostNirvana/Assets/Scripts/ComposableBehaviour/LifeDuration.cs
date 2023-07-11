using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Base;

namespace ComposableBehaviour {

public class LifeDuration : MonoBehaviour {
    [SerializeField] float durationSeconds;

    float timeEnabled;
    bool disposalRequested;

    void OnEnable() {
        timeEnabled = Time.time;
        disposalRequested = false;
    }

    void Update() {
        bool canDispose = !disposalRequested && Time.time - timeEnabled >= durationSeconds;
        if (canDispose) {
            Entity entity = GetComponentInParent<Entity>();
            entity.Dispose(); // should only be called once when lifetime ends
            disposalRequested = true;
        }
    }
}

}

