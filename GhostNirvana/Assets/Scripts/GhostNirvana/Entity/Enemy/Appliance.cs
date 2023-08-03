using AI;
using Base;
using UnityEngine;
using UnityEngine.Events;
using CombatSystem;
using NaughtyAttributes;

namespace GhostNirvana {

[RequireComponent(typeof(Status))]
public partial class Appliance : Enemy<Appliance.Input> {

    public enum States {
        Idle,
        BeforePossessed,
        Possessed,
        Collecting
    }

    public struct Input {
        public Vector3 desiredMovement;
    };

	[SerializeField] MovableAgentRuntimeSet allAppliances;

    #region Components
    [field:SerializeField, Required]
    public Collider PossessionDetection {get; private set; }

    public ApplianceStateMachine StateMachine {
        get; private set;
    }
    #endregion

    UnityEvent<Appliance, StateMachine<Appliance, Appliance.States>, Ghosty>
    OnPossessorDetected = new UnityEvent<Appliance, StateMachine<Appliance, States>, Ghosty>();

    [HideInInspector] public UnityEvent<Appliance> OnPossessionInterupt = new UnityEvent<Appliance>();
    [HideInInspector] public UnityEvent<Appliance> OnPossessionComplete = new UnityEvent<Appliance>();

    [SerializeField] StatusRuntimeSet allEnemyStatus;
    [field:SerializeField] public int Price {get; private set; }
    [BoxGroup("Movement"), Range(0, 720), SerializeField] float turnSpeed = 100;

    public bool IsPossessed => StateMachine.State == States.Possessed;
    public bool IsBeingPossessed => StateMachine.State == States.BeforePossessed;
    public bool CanPossess => !IsPossessed && !IsBeingPossessed;

    protected override void Awake() {
        base.Awake();
        StateMachine = GetComponent<ApplianceStateMachine>();
    }

    protected override void OnEnable() {
        base.OnEnable();
		allAppliances.Add(this);
        FreezePosition = true;
    }

    protected override void OnDisable() {
        base.OnDisable();
		allAppliances.Remove(this);
        IHurtResponder.DisconnectChildrenHurtboxes(this);
        IHitResponder.DisconnectChildrenHitboxes(this);
    }

    protected void Update() => PerformUpdate(StateMachine.RunUpdate);

    public void EventOnly_OnPossessionDetection(Collider other) {
        if (IsPossessed) return;
        Ghosty possessor = other.GetComponentInParent<Ghosty>();
        if (!possessor.CanPossess) return;
        OnPossessorDetected?.Invoke(this, StateMachine, possessor);
    }

    public void ApplianceCollectorOnly_Collect() => StateMachine.SetState(States.Collecting);
}

}
