using UnityEngine;
using Utils;

namespace ComposableBehaviour {

public class Orbiter : MonoBehaviour {
    [SerializeField] Vector3 rotationPerSeconds;
    float distanceToCenter;

    void OnEnable() {
        distanceToCenter = transform.localPosition.magnitude;
    }

    void Update() {
        Matrix4x4 localMatrix = Matrix4x4.TRS(
            transform.localPosition,
            transform.localRotation,
            transform.localScale
        );
        Matrix4x4 rotationThisFrame = Matrix4x4.Rotate(Quaternion.Euler(
            rotationPerSeconds.x * Time.deltaTime,
            rotationPerSeconds.y * Time.deltaTime,
            rotationPerSeconds.z * Time.deltaTime));

        Matrix4x4 localMatrixNextFrame = rotationThisFrame * localMatrix;
        transform.localPosition = localMatrixNextFrame.GetPosition();
        transform.localRotation = localMatrixNextFrame.ExtractRotation();
    }
}

}
