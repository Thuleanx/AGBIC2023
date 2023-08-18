using UnityEngine;
using Utils;
using NaughtyAttributes;

namespace ComposableBehaviour {

public class Wander : MonoBehaviour {
    [SerializeField] float containmentRadius;
    [SerializeField] float wanderAccelerationCircleRadius;
    [SerializeField] float wanderAccelerationCircleDistance;
    [SerializeField] float turnRatePerSecond;
    [SerializeField] Vector2 maxSpeedRandom;
    [SerializeField] float maxAcceleration;
    [SerializeField] float containmentMaxAcceleration;
    bool outsideContainmentLastFrame;
    float maxSpeed;

    Vector3 velocity;
    Vector3 cachedWanderDesiredDirection;
    Vector3 target;

    bool outsideOfContainment => (target - transform.position).sqrMagnitude > containmentRadius*containmentRadius;

    void Awake() {
        maxSpeed = Mathx.RandomRange(maxSpeedRandom);
    }

    protected void Start() {
        velocity = Random.onUnitSphere * maxSpeed;
        cachedWanderDesiredDirection = GenerateRandomWanterDirection();
        target = (transform.parent ?? transform).position;
    }

    void Update() {
        UpdateWanderDesiredVelocity();
        
        {
            Vector3 desiredVelocity = cachedWanderDesiredDirection * maxSpeed;
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

    Vector3 UpdateWanderDesiredVelocity() {
        Vector3 displacementToPivot = (target - transform.position);
        bool enterContainment = !outsideOfContainment && outsideContainmentLastFrame;
        if (outsideOfContainment) {
            cachedWanderDesiredDirection = GetContainmentDesiredVelocity();
        } else if (enterContainment || Mathx.RandomRange(0.0f,1.0f) < Time.deltaTime * turnRatePerSecond)
            cachedWanderDesiredDirection = GenerateRandomWanterDirection();
        return cachedWanderDesiredDirection;
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
        return wanderDirection.normalized;
    }
}

}
