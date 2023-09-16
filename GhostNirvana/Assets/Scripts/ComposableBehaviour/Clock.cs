using UnityEngine;
using UnityEngine.Events;

namespace ComposableBehaviour {

public class Clock : MonoBehaviour {
    [SerializeField] float ratePerSecond;
    [SerializeField] UnityEvent onClockTick;
    float progress;


    void OnEnable() => progress = 0;

    void Update() {
        progress += Time.deltaTime * ratePerSecond;
        if (progress >= 1) {
            onClockTick?.Invoke();
            progress--;
        }
    }
}

}
