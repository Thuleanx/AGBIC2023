using UnityEngine;
using UnityEngine.Events;

namespace GhostNirvana {

public class TemporaryBuffTracker : MonoBehaviour {
    [SerializeField] TemporaryBuff buff;

    [SerializeField] bool queryInitialState;
    [SerializeField] UnityEvent OnApply;
    [SerializeField] UnityEvent OnExpire;

    void OnEnable() {
        if (!buff) return;
        buff.OnApply.AddListener(_RunOnApplyEvent);
        buff.OnExpire.AddListener(_RunOnExpireEvent);
        if (queryInitialState) {
            if (buff.IsActive)  _RunOnApplyEvent();
            else                _RunOnExpireEvent();
        }
    }
    
    void OnDisable() {
        if (!buff) return;
        buff.OnApply.RemoveListener(_RunOnApplyEvent);
        buff.OnExpire.RemoveListener(_RunOnExpireEvent);
    }

    void _RunOnApplyEvent() => OnApply?.Invoke();
    void _RunOnExpireEvent() => OnExpire?.Invoke();
}

}
