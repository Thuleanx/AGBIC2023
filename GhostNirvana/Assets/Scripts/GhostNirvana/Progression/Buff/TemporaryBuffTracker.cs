using UnityEngine;
using UnityEngine.Events;

namespace GhostNirvana {

public class TemporaryBuffTracker : MonoBehaviour {
    [SerializeField] TemporaryBuff buff;

    [SerializeField] UnityEvent OnApply;
    [SerializeField] UnityEvent OnExpire;

    void OnEnable() {
        if (buff) {
            buff.OnApply.AddListener(_RunOnApplyEvent);
            buff.OnExpire.AddListener(_RunOnExpireEvent);
        }
    }
    
    void OnDisable() {
        if (buff) {
            buff.OnApply.RemoveListener(_RunOnApplyEvent);
            buff.OnExpire.RemoveListener(_RunOnExpireEvent);
        }
    }

    void _RunOnApplyEvent() => OnApply?.Invoke();
    void _RunOnExpireEvent() => OnExpire?.Invoke();
}

}
