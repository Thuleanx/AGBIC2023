using Base;
using AI;
using UnityEngine;
using Utils;
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
        AssignState<BigGhosty.BigGhostySeek>(BigGhosty.States.Seek);
        AssignState<BigGhosty.BigGhostySummon>(BigGhosty.States.Summon);
        AssignState<BigGhosty.BigGhostyWave>(BigGhosty.States.LaunchAttack);
        AssignState<BigGhosty.BigGhostyDeath>(BigGhosty.States.Death);
    }
}

public partial class BigGhosty {

public class BigGhostySeek : State<BigGhosty, BigGhosty.States> {
    public override States? Update(StateMachine<BigGhosty, States> stateMachine, BigGhosty agent) {
        if (stateMachine.CanEnter(States.Summon)) return States.Summon;
        if (stateMachine.CanEnter(States.LaunchAttack)) return States.LaunchAttack;

        Vector3 desiredVelocity = agent.input.desiredMovement * 
            agent.Status.BaseStats.MovementSpeed * agent.currentHaste;

        agent.Velocity = Mathx.AccelerateTowards(
            currentVelocity: agent.Velocity,
            desiredVelocity,
            acceleration: agent.currentAcceleration,
            maxSpeed: agent.currentMaxSpeed,
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

    public override bool CanEnter(StateMachine<BigGhosty, States> stateMachine, BigGhosty agent)
        => agent.canSummon && !agent.summonCooldown;

    public override void Begin(StateMachine<BigGhosty, States> stateMachine, BigGhosty agent) {
        stateMachine.Blackboard[KEY_summonAnimFinish] = false;
        agent.onSummonComplete.AddListener(OnSummonAnimationFinish);
        agent.onSummonChanneled.AddListener(OnSummonChanneled);
        agent.Anim.SetTrigger("Summon");
    }

    public override void End(StateMachine<BigGhosty, States> stateMachine, BigGhosty agent) {
        stateMachine.Blackboard.Remove(KEY_summonAnimFinish);
        agent.onSummonComplete.RemoveListener(OnSummonAnimationFinish);
        agent.onSummonChanneled.RemoveListener(OnSummonChanneled);
        agent.summonCooldown = Mathx.RandomRange(agent.currentSummonCooldown);
    }

    public override States? Update(StateMachine<BigGhosty, States> stateMachine, BigGhosty agent) {
        if ((bool) stateMachine.Blackboard[KEY_summonAnimFinish])
            return BigGhosty.States.Seek;

        // slow down
        agent.Velocity = Mathx.AccelerateTowards(
            currentVelocity: agent.Velocity,
            desiredVelocity: Vector3.zero,
            acceleration: agent.currentAcceleration,
            maxSpeed: agent.currentMaxSpeed,
            Time.deltaTime
        );
        if (agent.input.desiredMovement.sqrMagnitude > 0)
            agent.TurnToFace(agent.input.desiredMovement, agent.turnSpeedWhileSummoning);
        return null;
    }

    void PerformSummon(BigGhosty ghosty) {
        foreach (MovableAgent applianceAgent in ghosty.allAppliances) {
            Appliance appliance = applianceAgent as Appliance;
            if (!appliance) continue;

            float distance2BigGhost = (ghosty.transform.position - appliance.transform.position).sqrMagnitude;

            bool inSummonRange = distance2BigGhost <= ghosty.summonRange * ghosty.summonRange;
            bool shouldSummon = inSummonRange;
            if (!shouldSummon) continue;

            ghosty.SummonGhostyAt(position: appliance.transform.position);
        }
    }

    void OnSummonAnimationFinish(BigGhosty ghosty) => ghosty.StateMachine.Blackboard[KEY_summonAnimFinish] = true;
    void OnSummonChanneled(BigGhosty ghosty) => PerformSummon(ghosty);
}

public class BigGhostyWave : State<BigGhosty, BigGhosty.States> {
    static string KEY_waveAttackFinish = "WaveAnimationFinish";

    public override bool CanEnter(StateMachine<BigGhosty, States> stateMachine, BigGhosty agent)
        => !agent.attackCooldown;

    public override void Begin(StateMachine<BigGhosty, States> stateMachine, BigGhosty agent) {
        stateMachine.Blackboard[KEY_waveAttackFinish] = false;
        agent.onAttackComplete.AddListener(OnAttackAnimationFinish);
        agent.onAttackChanneled.AddListener(OnAttackChanneled);
        agent.Anim.SetTrigger("Attack");
    }

    public override void End(StateMachine<BigGhosty, States> stateMachine, BigGhosty agent) {
        agent.onAttackComplete.RemoveListener(OnAttackAnimationFinish);
        agent.onAttackChanneled.RemoveListener(OnAttackChanneled);

        agent.attackCooldown = Mathx.RandomRange(agent.currentAttackCooldown);
    }

    public override States? Update(StateMachine<BigGhosty, States> stateMachine, BigGhosty agent) {
        if ((bool) stateMachine.Blackboard[KEY_waveAttackFinish])
            return BigGhosty.States.Seek;

        // slow down
        agent.Velocity = Mathx.AccelerateTowards(
            currentVelocity: agent.Velocity,
            desiredVelocity: Vector3.zero,
            acceleration: agent.currentAcceleration,
            maxSpeed: agent.currentMaxSpeed,
            Time.deltaTime
        );

        if (agent.input.desiredMovement.sqrMagnitude > 0)
            agent.TurnToFace(agent.input.desiredMovement, agent.turnSpeedWhileAttacking);

        return null;
    }

    void PerformAttack(BigGhosty ghosty) {
        ghosty.AttackAt(direction: ghosty.transform.rotation * Vector3.forward);
    }

    void OnAttackAnimationFinish(BigGhosty ghosty) => ghosty.StateMachine.Blackboard[KEY_waveAttackFinish] = true;
    void OnAttackChanneled(BigGhosty ghosty) => PerformAttack(ghosty);
}

public class BigGhostyDeath : State<BigGhosty, BigGhosty.States> {
    public override void Begin(StateMachine<BigGhosty, States> stateMachine, BigGhosty agent) {
        if (agent.onDeathVFX)
            ObjectPoolManager.Instance?.Borrow(App.GetActiveScene(),
                agent.onDeathVFX.transform,
                agent.transform.position
            );
		agent.Dispose();
	}
}

}
}
