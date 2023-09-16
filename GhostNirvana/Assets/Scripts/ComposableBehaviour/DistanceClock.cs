using UnityEngine;
using UnityEngine.Events;

namespace ComposableBehaviour {

public class DistanceClock : MonoBehaviour {
    [SerializeField] float ratePerUnit;
    [SerializeField] UnityEvent<Vector3> onClockTick;
    float progress;
    Vector3 lastPos;

    void OnEnable() {
        progress = 0;
        lastPos = transform.position;
    }

    void Update() {
        Vector3 currentPos = transform.position;
        float distanceTravelled = ((currentPos - lastPos).magnitude) * ratePerUnit;
        while (progress + distanceTravelled >= 1) {
            float t = (1 - progress) / distanceTravelled;

            Vector3 pos = Vector3.Lerp(lastPos, currentPos, t);

            onClockTick?.Invoke(pos);

            lastPos = pos;
            distanceTravelled *= 1 - t;
            progress = 0;
        }
        progress += distanceTravelled;
        lastPos = currentPos;
    }
}

}
