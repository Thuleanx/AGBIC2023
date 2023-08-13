using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using ScriptableBehaviour;

namespace GhostNirvana {
    public class ActivateAtTime : MonoBehaviour {
        [SerializeField] UnityEvent trigger;
        [SerializeField] ScriptableFloat time;
        [SerializeField] float timeToTrigger;

        float lastTime = -1;

        void Update() {
            bool shouldTrigger = lastTime < timeToTrigger && time.Value >= timeToTrigger;
            if (shouldTrigger) {
                trigger?.Invoke();
            }
            lastTime = time.Value;
        }
    }
}
