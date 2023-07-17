using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI;
using Danmaku;
using Thuleanx.Utils;
using Optimization;

namespace GhostNirvana {

[RequireComponent(typeof(Miyu))]
public class MiyuStateMachine : StateMachine<Miyu, Miyu.States> {

    public Miyu Miyu { get; private set; }

    void Awake() {
        Miyu = GetComponent<Miyu>();

        ConstructMachine(agent: Miyu, defaultState: Miyu.States.Grounded);
    }

    public override void Construct() {
        AssignState<Miyu.MiyuGrounded>(Miyu.States.Grounded);
    }
}

public partial class Miyu {

public class MiyuGrounded : State<Miyu, Miyu.States> {
    public override Miyu.States? Update(StateMachine<Miyu, Miyu.States> stateMachine, Miyu miyu) {
        Vector3 desiredVelocity = miyu.input.desiredMovement * miyu.movementSpeed.Value;

        miyu.Velocity = Mathx.Damp(Vector3.Lerp, miyu.Velocity, desiredVelocity,
                                    (miyu.Velocity.sqrMagnitude > desiredVelocity.sqrMagnitude) ? miyu.deccelerationAlpha : miyu.accelerationAlpha, Time.deltaTime);

        bool isNotStationary = miyu.Velocity != Vector3.zero;
        if (isNotStationary)
            miyu.TurnToFace(miyu.Velocity, miyu.turnSpeed);

        float lastAttackTime = (float) (stateMachine.Blackboard["lastAttackTime"] ?? 0.0f);
        float timeSinceLastAttack = (Time.time - lastAttackTime);
        bool canAttack = timeSinceLastAttack * miyu.attackSpeed.Value > 1;

        if (canAttack) {
            stateMachine.Blackboard["lastAttackTime"] = Time.time;

            Vector3 targetDirection = miyu.input.targetPositionWS - miyu.transform.position;
            targetDirection.y = 0;
            if (targetDirection.sqrMagnitude == 0) 
                targetDirection = miyu.transform.forward;

            targetDirection.Normalize();

            miyu.ShootProjectile(targetDirection);
        }

        return null;
    }
}

}

}
