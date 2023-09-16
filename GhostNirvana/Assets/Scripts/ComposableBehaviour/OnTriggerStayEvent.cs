using UnityEngine;
using UnityEngine.Events;

namespace GhostNirvana {
    public class OnTriggerStayEvent : MonoBehaviour {
        [SerializeField] UnityEvent triggerEvent;

        public void OnTriggerStay(Collider collider) => triggerEvent?.Invoke();
    }
}
