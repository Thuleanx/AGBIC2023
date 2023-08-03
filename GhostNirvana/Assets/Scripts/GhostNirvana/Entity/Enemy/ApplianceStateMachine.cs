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

    void Start() => Init();

    public override void Construct() {
        AssignState<Appliance.ApplianceIdle>(Appliance.States.Idle);
        AssignState<Appliance.ApplianceBeforePossessed>(Appliance.States.BeforePossessed);
        AssignState<Appliance.AppliancePossessed>(Appliance.States.Possessed);
        AssignState<Appliance.ApplianceCollecting>(Appliance.States.Collecting);
    }
}

public partial class Appliance {

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
    }
}

public class AppliancePossessed : State<Appliance, Appliance.States> {
    public override void Begin(StateMachine<Appliance, States> stateMachine, Appliance agent) {
        agent.Status.HealToFull();
        agent.Status.OnDeath.AddListener(OnDeath);
        agent.allEnemyStatus.Add(agent.Status);
        IHurtResponder.ConnectChildrenHurtboxes(agent);
        IHitResponder.ConnectChildrenHitboxes(agent);
    }

    public override Appliance.States? Update(
        StateMachine<Appliance, States> stateMachine, Appliance agent) {

        Vector3 desiredVelocity = agent.input.desiredMovement * agent.Status.BaseStats.MovementSpeed;

        agent.Velocity = Mathx.Damp(Vector3.Lerp, agent.Velocity, desiredVelocity,
                              (agent.Velocity.sqrMagnitude > desiredVelocity.sqrMagnitude)
                              ? agent.Status.BaseStats.DeccelerationAlpha : agent.Status.BaseStats.AccelerationAlpha, Time.deltaTime);

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
        agent.Velocity = Vector3.zero;
        agent.allEnemyStatus.Remove(agent.Status);
        IHurtResponder.DisconnectChildrenHurtboxes(agent);
        IHitResponder.DisconnectChildrenHitboxes(agent);
    }

    void OnDeath(Status status) {
        if (status.Owner is Appliance) {
            Appliance agent = (status.Owner as Appliance);
            // We directly retrieve and transition the state. This is not ideal
            agent.StateMachine.SetState(States.Idle);
        }
    }
}

public class ApplianceBeforePossessed : State<Appliance, Appliance.States> {
    public override void Begin(StateMachine<Appliance, States> stateMachine, Appliance agent) {
        agent.OnPossessionInterupt.AddListener(OnPossessionInterupted);
        agent.OnPossessionComplete.AddListener(OnPossessionCompleted);
    }

    public override void End(StateMachine<Appliance, States> stateMachine, Appliance agent) {
        agent.OnPossessionInterupt.RemoveListener(OnPossessionInterupted);
        agent.OnPossessionComplete.RemoveListener(OnPossessionCompleted);
    }

    void OnPossessionInterupted(Appliance appliance) 
        => appliance.StateMachine.SetState(States.Idle);

    void OnPossessionCompleted(Appliance appliance)
        => appliance.StateMachine.SetState(States.Possessed);
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
