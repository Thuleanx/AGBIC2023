using System.Collections;
using Base;
using AI;
using UnityEngine;
using Utils;
using CombatSystem;
using Optimization;

namespace GhostNirvana {
[RequireComponent(typeof(BigGhosty))]
public class BigGhostyStateMachine : StateMachine<BigGhosty, BigGhosty.States> {
    public BigGhosty Ghosty {get; private set;}

    void Awake() {
        Ghosty = GetComponent<BigGhosty>();
        ConstructMachine(agent: Ghosty, defaultState: BigGhosty.States.Seek);
    }

    public override void Construct() {
    }
}

public partial class BigGhosty {

public class BigGhostySeek : State<BigGhosty, BigGhosty.States> {
    public override States? Update(StateMachine<BigGhosty, States> stateMachine, BigGhosty agent) {
        Vector3 desiredVelocity = agent.input.desiredMovement * agent.Status.BaseStats.MovementSpeed;

        agent.Velocity = Mathx.AccelerateTowards(
            currentVelocity: agent.Velocity,
            desiredVelocity,
            acceleration: agent.Status.BaseStats.Acceleration,
            maxSpeed: agent.Status.BaseStats.MovementSpeed,
            Time.deltaTime
        );

        if (!Miyu.Instance || !Miyu.Instance.gameObject) return null;

        // TODO: rid of magic number
        float hitboxCheckingDistance = 3;
        bool closeToPlayer = (Miyu.Instance.transform.position - agent.transform.position).sqrMagnitude < hitboxCheckingDistance * hitboxCheckingDistance;
        if (closeToPlayer) agent.CheckForHits();

        bool isNotStationary = agent.Velocity != Vector3.zero;
        if (isNotStationary) agent.TurnToFace(agent.Velocity, agent.turnSpeed);

        return null;
    }
}

public class BigGhostySummon : State<BigGhosty, BigGhosty.States> {
    static string KEY_summonAnimFinish = "SummonAnimationFinished";

    public override void Begin(StateMachine<BigGhosty, States> stateMachine, BigGhosty agent) {
        stateMachine.Blackboard[KEY_summonAnimFinish] = false;
        agent.onSummonComplete.AddListener(OnSummonAnimationFinish);
    }

    public override void End(StateMachine<BigGhosty, States> stateMachine, BigGhosty agent) {
        stateMachine.Blackboard.Remove(KEY_summonAnimFinish);
        agent.onSummonComplete.RemoveListener(OnSummonAnimationFinish);
    }

    public override States? Update(StateMachine<BigGhosty, States> stateMachine, BigGhosty agent) {
        if ((bool) stateMachine.Blackboard[KEY_summonAnimFinish])
            return BigGhosty.States.Seek;
        // slow down
        agent.Velocity = Mathx.AccelerateTowards(
            currentVelocity: agent.Velocity,
            desiredVelocity: Vector3.zero,
            acceleration: agent.Status.BaseStats.Acceleration,
            maxSpeed: agent.Status.BaseStats.MovementSpeed,
            Time.deltaTime
        );
        return null;
    }

    void PerformSummon(BigGhosty ghosty) {
        foreach (Status applianceStatus in ghosty.allAppliances) {
            Appliance appliance = applianceStatus.GetComponent<Appliance>();
            if (!appliance) continue;

            float distance2BigGhost = (ghosty.transform.position - appliance.transform.position).sqrMagnitude;

            bool inSummonRange = distance2BigGhost <= ghosty.summonRange * ghosty.summonRange;
            bool shouldSummon = inSummonRange && !appliance.IsPossessedByGhost;
            if (!shouldSummon) continue;

            ghosty.SummonGhostyAt(position: appliance.transform.position);
        }
    }

    void OnSummonAnimationFinish(BigGhosty ghosty) {
        ghosty.StateMachine.Blackboard[KEY_summonAnimFinish] = true;
        PerformSummon(ghosty);
    }
}

public class BigGhostyWave : State<BigGhosty, BigGhosty.States> {
}

public class BigGhostyDeath : State<BigGhosty, BigGhosty.States> {
    public override void Begin(StateMachine<BigGhosty, States> stateMachine, BigGhosty agent) {
		agent.Dispose();
	}
}

}
}
