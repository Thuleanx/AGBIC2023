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

    UnityEvent<Appliance, StateMachine<Appliance, Appliance.States>, Entity> OnPossessorDetected = new UnityEvent<Appliance, StateMachine<Appliance, States>, Entity>();

    [SerializeField] StatusRuntimeSet allEnemyStatus;
    [field:SerializeField] public int Price {get; private set; }

    public bool IsPossessed => StateMachine.State == States.Possessed;

    protected override void Awake() {
        base.Awake();
        StateMachine = GetComponent<ApplianceStateMachine>();
    }

    protected override void OnEnable() {
        base.OnEnable();
		allAppliances.Add(this);
        IHurtResponder.ConnectChildrenHurtboxes(this);
        IHitResponder.ConnectChildrenHitboxes(this);
        FreezePosition = true;
    }

    protected override void OnDisable() {
        base.OnDisable();
		allAppliances.Remove(this);
        IHurtResponder.DisconnectChildrenHurtboxes(this);
        IHitResponder.ConnectChildrenHitboxes(this);
    }

    protected void Update() => PerformUpdate(StateMachine.RunUpdate);

    public void EventOnly_OnPossessionDetection(Collider other) {
        Entity entity = other.GetComponentInParent<Entity>();
        OnPossessorDetected?.Invoke(this, StateMachine, entity);
    }

    public void ApplianceCollectorOnly_Collect() => StateMachine.SetState(States.Collecting);
}

}
