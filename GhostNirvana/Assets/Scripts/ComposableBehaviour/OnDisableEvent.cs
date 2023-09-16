using UnityEngine;
using UnityEngine.Events;

namespace GhostNirvana {
    public class OnDisableEvent : MonoBehaviour {
        [SerializeField] UnityEvent triggerEvent;

        public void OnDisable() => triggerEvent?.Invoke();
    }
}
