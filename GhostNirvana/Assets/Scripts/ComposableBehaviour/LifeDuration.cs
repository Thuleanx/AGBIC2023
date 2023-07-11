using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Base;

namespace ComposableBehaviour {

public class LifeDuration : MonoBehaviour {
    [SerializeField] float durationSeconds;

    float timeEnabled;

    void OnEnable() {
        timeEnabled = Time.time;
        this.enabled = true;
    }

    void Update() {
        if (Time.time - timeEnabled >= durationSeconds) {
            Entity entity = GetComponentInParent<Entity>();
            entity.Dispose(); // should only be called once when lifetime ends

            this.enabled = false;
        }
    }
}

}

