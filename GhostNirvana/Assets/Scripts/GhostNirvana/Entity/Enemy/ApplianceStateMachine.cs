using Base;
using AI;
using CombatSystem;
using UnityEngine;
using Utils;
using System.Collections;

namespace GhostNirvana {
public class ApplianceStateMachine : StateMachine<Appliance, Appliance.States> {
    public Appliance Appliance {get; private set;}

    void Awake() {
        Appliance = GetComponent<Appliance>();
        ConstructMachine(agent: Appliance, defaultState: Appliance.States.Idle);
    }

    public override void Construct() {
        AssignState<Appliance.ApplianceIdle>(Appliance.States.Idle);
        AssignState<Appliance.ApplianceBeforePossessed>(Appliance.States.BeforePossessed);
        AssignState<Appliance.AppliancePossessed>(Appliance.States.Possessed);
        AssignState<Appliance.ApplianceCollecting>(Appliance.States.Collecting);
    }
}

public partial class Appliance {

const string BK_GhostPossessing = "ghostPossessing";
const string BK_GhostHP = "ghostHP";
const string BK_LastHit = "applianceLastHit";
const string BK_AppliancePossessionOrganicallyExited = "appliancePossessionOrganicallyExited";

public class ApplianceIdle : State<Appliance, Appliance.States> {
    public override void Begin(StateMachine<Appliance, States> stateMachine, Appliance agent) {
        agent.OnPossessorDetected.AddListener(OnPossessionDetected);
        agent.PossessionDetection.enabled = true;
        agent.FreezePosition = true;
    }

    public override void End(StateMachine<Appliance, States> stateMachine, Appliance agent) {
        agent.OnPossessorDetected.RemoveListener(OnPossessionDetected);
        agent.PossessionDetection.enabled = false;
        agent.FreezePosition = false;
    }

    void OnPossessionDetected(Appliance agent, StateMachine<Appliance, States> stateMachine, Ghosty possessor) {
        possessor.ApplianceOnly_Possess(agent);
        stateMachine.SetState(States.BeforePossessed);

        stateMachine.Blackboard[BK_GhostPossessing] = possessor;
    }
}

public class ApplianceBeforePossessed : State<Appliance, Appliance.States> {
    public override void Begin(StateMachine<Appliance, States> stateMachine, Appliance agent) {
        agent.OnPossessionInterupt.AddListener(OnPossessionInterupted);
        agent.OnPossessionComplete.AddListener(OnPossessionCompleted);
        stateMachine.Blackboard[BK_AppliancePossessionOrganicallyExited] = false;
    }

    public override void End(StateMachine<Appliance, States> stateMachine, Appliance agent) {
        agent.OnPossessionInterupt.RemoveListener(OnPossessionInterupted);
        agent.OnPossessionComplete.RemoveListener(OnPossessionCompleted);

        bool organicallyExited = (bool) stateMachine.Blackboard[BK_AppliancePossessionOrganicallyExited];
        stateMachine.Blackboard.Remove(BK_AppliancePossessionOrganicallyExited);

        if (organicallyExited) return;

        Ghosty ghost = stateMachine.Blackboard[BK_GhostPossessing] as Ghosty;
        ghost.OnPossessionInterupt?.Invoke(ghost);

        stateMachine.Blackboard.Remove(BK_GhostPossessing);
    }

    void OnPossessionInterupted(Appliance appliance) {
        appliance.StateMachine.SetState(States.Idle);
        appliance.StateMachine.Blackboard.Remove(BK_GhostPossessing);
        appliance.StateMachine.Blackboard[BK_AppliancePossessionOrganicallyExited] = true;
    }

    void OnPossessionCompleted(Appliance appliance) {
        appliance.StateMachine.Blackboard[BK_AppliancePossessionOrganicallyExited] = true;

        Ghosty ghost = appliance.StateMachine.Blackboard[BK_GhostPossessing] as Ghosty;
        appliance.StateMachine.Blackboard[BK_GhostHP] = ghost.Status.Health;
        ghost.gameObject.SetActive(false);

        appliance.StateMachine.SetState(States.Possessed);
    }
}

public class AppliancePossessed : State<Appliance, Appliance.States> {
    public override void Begin(StateMachine<Appliance, States> stateMachine, Appliance agent) {
        agent.Status.HealToFull();
        agent.Status.OnDeath.AddListener(OnDeath);
        agent.allEnemyStatus.Add(agent.Status);
        agent.OnDamage.AddListener(OnTakeDamage);
        IHurtResponder.ConnectChildrenHurtboxes(agent);
        IHitResponder.ConnectChildrenHitboxes(agent);
    }

    public override Appliance.States? Update(
        StateMachine<Appliance, States> stateMachine, Appliance agent) {

        Vector3 desiredVelocity = agent.input.desiredMovement * agent.Status.BaseStats.MovementSpeed;

        agent.Velocity = Mathx.AccelerateTowards(
            currentVelocity: agent.Velocity,
            desiredVelocity,
            acceleration: agent.Status.BaseStats.Acceleration,
            maxSpeed: agent.Status.BaseStats.MovementSpeed,
            Time.deltaTime
        );

        float hitboxCheckingDistance = 2;
        bool closeToPlayer = (Miyu.Instance.transform.position - agent.transform.position).sqrMagnitude < hitboxCheckingDistance;
        if (closeToPlayer)
            agent.CheckForHits();

        bool isNotStationary = agent.Velocity != Vector3.zero;
        if (isNotStationary)
            agent.TurnToFace(agent.Velocity, agent.turnSpeed);

        return null;
    }

    public override void End(StateMachine<Appliance, States> stateMachine, Appliance agent) {
        agent.Status.OnDeath.RemoveListener(OnDeath);
        agent.OnDamage.RemoveListener(OnTakeDamage);
        agent.Velocity = Vector3.zero;
        agent.allEnemyStatus.Remove(agent.Status);
        IHurtResponder.DisconnectChildrenHurtboxes(agent);
        IHitResponder.DisconnectChildrenHitboxes(agent);
    }

    void OnTakeDamage(IHurtable hurtable, int damageAmount, DamageType damageType, Hit hit) {
        if (!(hurtable is Appliance)) return;

        Appliance appliance = hurtable as Appliance;
        appliance.StateMachine.Blackboard[BK_LastHit] = hit;
    }

    void OnDeath(Status status) {
        if (!(status.Owner is Appliance)) return;

        Appliance appliance = (status.Owner as Appliance);
        Ghosty ghost = appliance.StateMachine.Blackboard[BK_GhostPossessing] as Ghosty;

        Hit lastHit = (Hit) appliance.StateMachine.Blackboard[BK_LastHit];
        PushGhostOutOfAppliance(ghost, appliance, lastHit);

        appliance.possessionCooldown = appliance.cooldownAfterPossession;
        appliance.StateMachine.Blackboard.Remove(BK_GhostPossessing);
        appliance.StateMachine.Blackboard.Remove(BK_GhostHP);
        appliance.StateMachine.Blackboard.Remove(BK_LastHit);

        // We directly retrieve and transition the state. This is not ideal
        appliance.StateMachine.SetState(States.Idle);
    }

    void PushGhostOutOfAppliance(Ghosty ghost, Appliance appliance, Hit hit) {
        ghost.transform.position = appliance.transform.position;

        ghost.gameObject.SetActive(true);

        (ghost as IKnockbackable).ApplyKnockback(
            appliance.knockbackOnEjection, dir: -hit.Normal);

        int ghostHealthBeforePossession = (int) appliance.StateMachine.Blackboard[BK_GhostHP];
        ghost.Status.SetHealth(ghostHealthBeforePossession);
    }
}

public class ApplianceCollecting : State<Appliance, Appliance.States> {
    public override void Begin(StateMachine<Appliance, States> stateMachine, Appliance agent) {
        agent.FreezePosition = true;
    }

    public override IEnumerator Coroutine(StateMachine<Appliance, States> stateMachine, Appliance agent) {
        yield return null;
        agent.FreezePosition = false;
        agent.Dispose();
    }
}

}
}
