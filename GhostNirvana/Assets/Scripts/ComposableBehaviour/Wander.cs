using UnityEngine;
using Utils;

namespace ComposableBehaviour {

public class Wander : MonoBehaviour {
    [SerializeField] float containmentRadius;
    [SerializeField] float wanderAccelerationCircleRadius;
    [SerializeField] float wanderAccelerationCircleDistance;
    [SerializeField] float turnRatePerSecond;
    [SerializeField] Vector2 maxSpeedRandom;
    [SerializeField] float maxAcceleration;
    [SerializeField] float containmentMaxAcceleration;
    [SerializeField, Range(0, 1.0f)] float containmentStrength;
    bool outsideContainmentLastFrame;
    float maxSpeed;

    Vector3 velocity;
    Vector3 cachedWanderDesiredVelocity;
    Vector3 cachedContainmentDesiredVelocity;
    Vector3 originalPos;
    Vector3 target => (AttachedTransform ?? transform.parent ?? transform).position + Offset;

    Vector3 desiredDirection => Vector3.Lerp(cachedWanderDesiredVelocity, cachedContainmentDesiredVelocity, containmentStrength).normalized;
    /* Vector3 target => Offset; */

    public Transform AttachedTransform;
    public Vector3 Offset;

    bool outsideOfContainment => (target - transform.position).sqrMagnitude > containmentRadius*containmentRadius;

    void Awake() {
        maxSpeed = Mathx.RandomRange(maxSpeedRandom);
    }

    protected void Start() {
        velocity = Random.onUnitSphere * maxSpeed;
        cachedWanderDesiredVelocity = GenerateRandomWanterDirection();
    }

    void Update() {
        UpdateWanderDesiredVelocity();
        
        {
            Vector3 desiredVelocity = desiredDirection * maxSpeed;
            Vector3 steering = desiredVelocity - velocity;

            Debug.DrawRay(transform.position, desiredVelocity.normalized * 2, Color.red);

            steering = Vector3.ClampMagnitude(steering, outsideOfContainment ? containmentMaxAcceleration : maxAcceleration);
            velocity = Vector3.ClampMagnitude(velocity + steering * Time.deltaTime, maxSpeed);
        }

        transform.position += velocity * Time.deltaTime;
        transform.rotation = Quaternion.LookRotation(velocity, upwards: transform.up);
        Debug.DrawRay(transform.position,velocity.normalized * 2, Color.green);
        
        outsideContainmentLastFrame = outsideOfContainment;
    }

    void UpdateWanderDesiredVelocity() {
        Vector3 displacementToPivot = (target - transform.position);
        bool enterContainment = !outsideOfContainment && outsideContainmentLastFrame;

        cachedContainmentDesiredVelocity = Vector3.zero;
        if (outsideOfContainment)   cachedContainmentDesiredVelocity = GetContainmentDesiredVelocity();

        if (enterContainment || Mathx.RandomRange(0.0f,1.0f) < Time.deltaTime * turnRatePerSecond)
            cachedWanderDesiredVelocity = GenerateRandomWanterDirection();
    }
    
    Vector3 GetContainmentDesiredVelocity() {
        Vector3 displacementToPivot = (target - transform.position);
        float diffFromContainment = Mathf.Clamp01((displacementToPivot.magnitude - containmentRadius) / 5);
        return Vector3.Lerp(displacementToPivot.normalized * maxSpeed + velocity, displacementToPivot.normalized * maxSpeed - velocity, diffFromContainment);
        /* return displacementToPivot.normalized * maxSpeed + Vector3.Lerp(velocity, -velocity, Mathf.Clamp01(diffFromContainment)); */
    }

    Vector3 GenerateRandomWanterDirection() {
        Vector3 wanderCircleCenter = velocity.sqrMagnitude > 0.001 ?
            velocity.normalized * wanderAccelerationCircleDistance : Vector3.zero;

        Vector3 displacement = Random.insideUnitCircle * wanderAccelerationCircleRadius;
        displacement = Quaternion.LookRotation(velocity, transform.up) * displacement;

        Vector3 wanderDirection = wanderCircleCenter + displacement;
        return wanderDirection;
    }
}

}
