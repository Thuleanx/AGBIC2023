using UnityEngine;
using UnityEngine.Events;

namespace ComposableBehaviour {

public class FallingAppliance : MonoBehaviour {
    [SerializeField] float gravity;
    [SerializeField] UnityEvent onGroundHit;
    Vector3 velocity;

    void OnEnable() {
        velocity = Vector3.zero;
    }

    void Update() {
        velocity += gravity * Time.deltaTime * Vector3.down;
        transform.position += velocity * Time.deltaTime;

        if (transform.position.y < 0) {
            transform.position = new Vector3(
                transform.position.x,
                0,
                transform.position.z
            );
            onGroundHit?.Invoke();
        }
    }
}

}
