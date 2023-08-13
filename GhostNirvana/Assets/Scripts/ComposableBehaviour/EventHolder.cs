using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using NaughtyAttributes;

namespace ComposableBehaviour {

public class EventHolder : MonoBehaviour {
    [SerializeField] bool triggerOnce;
    [SerializeField] UnityEvent OnTrigger;
    [SerializeField, Min(0)] float delaySeconds;
	[SerializeField, ReadOnly] bool triggered;

    void OnEnable() => triggered = false;

    public void Trigger() {
        if (triggered && triggerOnce) return;
        if (delaySeconds > 0)   StartCoroutine(InvokeWithDelay());
        else                    OnTrigger?.Invoke();
        triggered = true;
    }

    IEnumerator InvokeWithDelay() {
        yield return new WaitForSecondsRealtime(delaySeconds);
        OnTrigger?.Invoke();
    }
}

}
