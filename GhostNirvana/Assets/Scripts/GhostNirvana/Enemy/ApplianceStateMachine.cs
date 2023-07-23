using Base;
using AI;
using CombatSystem;
using UnityEngine;
using Utils;

namespace GhostNirvana {
public class ApplianceStateMachine : StateMachine<Appliance, Appliance.States> {
    public Appliance Appliance {get; private set;}

    void Awake() {
        Appliance = GetComponent<Appliance>();
        ConstructMachine(agent: Appliance, defaultState: Appliance.States.Idle);
    }

    void Start() => Init();

    public override void Construct() {
        AssignState<Appliance.ApplianceIdle>(Appliance.States.Idle);
        AssignState<Appliance.AppliancePossessed>(Appliance.States.Possessed);
    }
}

public partial class Appliance {

public class ApplianceIdle : State<Appliance, Appliance.States> {
    public override void Begin(StateMachine<Appliance, States> stateMachine, Appliance agent) {
        agent.OnPossessorDetected.AddListener(OnPossessionDetected);
        agent.PossessionDetection.enabled = true;
    }

    public override void End(StateMachine<Appliance, States> stateMachine, Appliance agent) {
        agent.OnPossessorDetected.RemoveListener(OnPossessionDetected);
        agent.PossessionDetection.enabled = false;
    }

    void OnPossessionDetected(Appliance agent, StateMachine<Appliance, States> stateMachine, Entity possessor) {
        possessor.Dispose();
        stateMachine.SetState(States.Possessed);
    }
}

public class AppliancePossessed : State<Appliance, Appliance.States> {
    public override void Begin(StateMachine<Appliance, States> stateMachine, Appliance agent) {
        agent.Status.HealToFull();
        agent.Status.OnDeath.AddListener(OnDeath);
        HealthBarManager.Instance?.AddStatus(agent.Status);
    }

    public override Appliance.States? Update(
        StateMachine<Appliance, States> stateMachine, Appliance agent) {

        Vector3 desiredVelocity = agent.input.desiredMovement * agent.Status.BaseStats.MovementSpeed;

        agent.Velocity = Mathx.Damp(Vector3.Lerp, agent.Velocity, desiredVelocity,
                              (agent.Velocity.sqrMagnitude > desiredVelocity.sqrMagnitude)
                              ? agent.Status.BaseStats.DeccelerationAlpha : agent.Status.BaseStats.AccelerationAlpha, Time.deltaTime);

        float hitboxCheckingDistance = 2;
        bool closeToPlayer = (Miyu.Instance.transform.position - agent.transform.position).sqrMagnitude < hitboxCheckingDistance;
        if (closeToPlayer) foreach (Hitbox hitbox in agent.hitboxes)
            hitbox.CheckForHits();

        return null;
    }

    public override void End(StateMachine<Appliance, States> stateMachine, Appliance agent) {
        agent.Status.OnDeath.RemoveListener(OnDeath);
        agent.Velocity = Vector3.zero;
        HealthBarManager.Instance?.RemoveStatus(agent.Status);
    }

    void OnDeath(Status status) {
        if (status.Owner is Appliance) {
            Appliance agent = (status.Owner as Appliance);
            // We directly retrieve and transition the state. This is not ideal
            agent.StateMachine.SetState(States.Idle);
        }
    }
}

}
}
