using UnityEngine;
using UnityEngine.Events;
using Utils;
using NaughtyAttributes;

namespace ComposableBehaviour {

public class JointChild : MonoBehaviour {
    Vector3 positionLastFrameWS;
    Vector3 velocityLS;

    Vector3 initialLocalPosition;
    float initialDistance;

    [SerializeField] float springForce;
    [SerializeField] float springDamping;
    [SerializeField] float gravity;
    [SerializeField] float parentRotationDamp = 15f;
    [SerializeField] bool reverseRight;
    [SerializeField] bool debug;

    [SerializeField, Range(0, 180.0f)] float phiLimit;

    [SerializeField] Matrix4x4 parentInitialLocalTransform;

    Vector4 toPoint(Vector3 pt) => new Vector4(pt.x, pt.y, pt.z, 1);

    Vector3 toLocalPoint(Vector3 posWS) => parentInitialLocalTransform * toPoint(transform.parent.parent.InverseTransformPoint(posWS));
    Vector3 toLocalVector(Vector3 posWS) => parentInitialLocalTransform * transform.parent.parent.InverseTransformVector(posWS);

    Vector3 toWorldPoint(Vector3 posLS) => transform.parent.parent.TransformPoint(parentInitialLocalTransform.inverse * toPoint(posLS));
    Vector3 toWorldVector(Vector3 posLS) => transform.parent.parent.TransformVector(parentInitialLocalTransform.inverse * posLS);

    void OnEnable() {
        positionLastFrameWS = transform.position;
        parentInitialLocalTransform = transform.parent.worldToLocalMatrix * transform.parent.parent.localToWorldMatrix; // grandparentLocal > parentLocal

        initialLocalPosition = toLocalPoint(transform.position);
        initialDistance = initialLocalPosition.magnitude;
    }

    void LateUpdate() {
        Vector3 downLS = toLocalVector(Vector3.down);

        Vector3 posLS = toLocalPoint(positionLastFrameWS);
        Vector3 initialToConstrainedLS = posLS - initialLocalPosition;

        Vector3 accelerationLS = gravity * downLS - initialToConstrainedLS * springForce;

        velocityLS += accelerationLS * Time.deltaTime;

        // damping
        float damp = Time.deltaTime * springDamping;
        if (damp > 1)   velocityLS = Vector3.zero;
        else            velocityLS *= (1 - damp);

        Vector3 framePosLS = ConstrainPosition(velocityLS * Time.deltaTime + posLS);
        /* Vector3 framePosLS = initialLocalPosition; */
        Vector3 framePosWS = toWorldPoint(framePosLS);

        positionLastFrameWS = framePosWS;
        transform.position = framePosWS;

        Vector3 parentRightWS = (transform.parent.position - framePosWS) * (reverseRight ? -1 : 1);
        /* Quaternion targetParentRotation = Quaternion.LookRotation(parentRightWS, transform.parent.up) * Quaternion.Euler(0, -90.0f, 0); */

        /* transform.parent.rotation = Mathx.Damp(Quaternion.Slerp, transform.parent.rotation, targetParentRotation, parentRotationDamp, Time.deltaTime); */
        /* transform.parent.rotation = targetParentRotation; */
        transform.parent.right = parentRightWS;
    }

    Vector3 ConstrainPosition(Vector3 pos) {
        pos = pos * initialDistance / pos.magnitude;

        float dyz = Mathf.Sqrt(pos.y * pos.y + pos.z * pos.z);

        float phi = Mathf.Atan2(dyz, pos.x) * Mathf.Rad2Deg;
        float centralPhiLimit = 0;

        phi += (reverseRight ? 0 : 180);
        phi += Mathf.Round((centralPhiLimit - phi) / 360.0f) * 360;
        phi = Mathf.Clamp(phi, -phiLimit, phiLimit);
        phi -= (reverseRight ? 0 : 180);

        float YZoverX = (Mathf.Abs(phi) < 0.01f) ? 0 : Mathf.Tan(phi * Mathf.Deg2Rad);

        Vector3 correctedPos = Vector3.right * pos.x + 
            (pos.y * Vector3.up + pos.z * Vector3.forward).normalized * (pos.x * YZoverX);

        correctedPos = correctedPos * initialDistance / correctedPos.magnitude;

        /* return correctedPos; */
        /* return initialLocalPosition; */
        /* return Vector3.right * (reverseRight ? 1 : -1); */
        return correctedPos;
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 0.05f);
    }
}

}
