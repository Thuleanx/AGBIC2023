using UnityEngine;
using UnityEngine.Events;

namespace GhostNirvana {

public class TemporaryBuffTracker : MonoBehaviour {
    [SerializeField] TemporaryBuff buff;

    [SerializeField] bool queryInitialState;
    [SerializeField] UnityEvent OnApply;
    [SerializeField] UnityEvent OnExpire;
    bool previousFrameActive;

    void OnEnable() {
        if (!buff) return;
        if (queryInitialState) {
            if (buff.IsActive)  _RunOnApplyEvent();
            else                _RunOnExpireEvent();
        }
        previousFrameActive = buff.IsActive;
    }

    void Update() {
        if (previousFrameActive ^ buff.IsActive) {
            if (buff.IsActive)  _RunOnApplyEvent();
            else                _RunOnExpireEvent();
        }
        previousFrameActive = buff.IsActive;
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
