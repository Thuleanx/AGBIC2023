using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI;
using Danmaku;
using Utils;
using Optimization;

namespace GhostNirvana {

[RequireComponent(typeof(Miyu))]
public class MiyuStateMachine : StateMachine<Miyu, Miyu.States> {

    public Miyu Miyu { get; private set; }

    void Awake() {
        Miyu = GetComponent<Miyu>();

        ConstructMachine(agent: Miyu, defaultState: Miyu.States.Grounded);
    }

    void Start() => Init();

    public override void Construct() {
        AssignState<Miyu.MiyuGrounded>(Miyu.States.Grounded);
        AssignState<Miyu.MiyuDead>(Miyu.States.Dead);
    }
}

public partial class Miyu {

public class MiyuGrounded : State<Miyu, Miyu.States> {
    public override Miyu.States? Update(StateMachine<Miyu, Miyu.States> stateMachine, Miyu miyu) {
        Vector3 desiredVelocity = miyu.input.desiredMovement * miyu.movementSpeed.Value;

        miyu.Velocity = Mathx.Damp(Vector3.Lerp, miyu.Velocity, desiredVelocity,
                                    (miyu.Velocity.sqrMagnitude > desiredVelocity.sqrMagnitude) ? miyu.deccelerationAlpha : miyu.accelerationAlpha, Time.deltaTime);


        float lastAttackTime = (float) (stateMachine.Blackboard["lastAttackTime"] ?? 0.0f);
        float timeSinceLastAttack = (Time.time - lastAttackTime);

        bool outOfBullets = miyu.magazine.Value <= 0;
        bool canAttack = !outOfBullets && timeSinceLastAttack * miyu.attackSpeed.Value > 1;

        if (canAttack) {
            stateMachine.Blackboard["lastAttackTime"] = Time.time;


            Vector3 targetPos = miyu.input.targetPositionWS;
            if (targetPos == miyu.transform.position)
                targetPos = miyu.transform.position + miyu.transform.forward;

            miyu.ShootProjectile(miyu.input.targetPositionWS);
            miyu.magazine.Value--;

        } else if (outOfBullets){
            float timeLeftUntilReload = (float) (stateMachine.Blackboard["timeLeftUntilReload"] ?? 0.0f);

            // this was from previous reload or first reload
            if (timeLeftUntilReload <= 0)
                timeLeftUntilReload = 1 / miyu.reloadRate.Value;

            timeLeftUntilReload -= Time.deltaTime;

            // reload completed
            if (timeLeftUntilReload <= 0)
                miyu.magazine.Value = miyu.magazine.Limiter;

            stateMachine.Blackboard["timeLeftUntilReload"] = timeLeftUntilReload;
        }

        return null;
    }
}

public class MiyuDead : State<Miyu, Miyu.States> {
    public override void Begin(StateMachine<Miyu, States> stateMachine, Miyu agent) {
        agent.Velocity = Vector3.zero;
    }
}


}

}
