using UnityEngine;
using UnityEngine.Events;

namespace ComposableBehaviour {

public class UnityEventWrapper : MonoBehaviour {
    [System.Serializable]
    public class UnityEventCollider : UnityEvent<Collider> {}

    [field:SerializeField]
    public UnityEventCollider _OnTriggerEnter {get; private set; }

    void OnTriggerEnter(Collider collider) => _OnTriggerEnter?.Invoke(collider);
}

}
