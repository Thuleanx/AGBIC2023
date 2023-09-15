using UnityEngine;
using UnityEngine.Events;

namespace ComposableBehaviour {

public class UnityEventWrapper : MonoBehaviour {
    [System.Serializable]
    public class UnityEventCollider : UnityEvent<Collider> {}

    [field:SerializeField]
    public UnityEventCollider _OnTriggerEnter {get; private set; }

    [field:SerializeField]
    public UnityEvent _OnEnable {get; private set; }

	void OnEnable() => _OnEnable?.Invoke();
    void OnTriggerEnter(Collider collider) => _OnTriggerEnter?.Invoke(collider);
}

}
