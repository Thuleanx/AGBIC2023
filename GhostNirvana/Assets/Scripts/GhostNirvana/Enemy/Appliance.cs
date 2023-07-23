using AI;
using Base;
using UnityEngine;
using UnityEngine.Events;
using CombatSystem;
using NaughtyAttributes;

namespace GhostNirvana {

[RequireComponent(typeof(Status))]
public partial class Appliance : 
    PossessableAgent<Appliance.Input>, IHurtable, IHurtResponder {

    Entity IHurtResponder.Owner => this;

    public enum States {
        Idle,
        Possessed
    }

    public struct Input {
        public Vector3 desiredMovement;
    };

    #region Components
    public Status Status { get; private set; }
    [field:SerializeField, Required]
    public Collider PossessionDetection {get; private set; }

    public ApplianceStateMachine StateMachine {
        get; private set;
    }
    #endregion

    UnityEvent<Appliance, StateMachine<Appliance, Appliance.States>, Entity> OnPossessorDetected = new UnityEvent<Appliance, StateMachine<Appliance, States>, Entity>();

    protected override void Awake() {
        base.Awake();
        StateMachine = GetComponent<ApplianceStateMachine>();
        Status = GetComponent<Status>();
        Status.Owner = this;
    }

    void IHurtable.OnTakeDamage(float damageAmount, DamageType damageType, Hit hit)
        => Status.TakeDamage(damageAmount);

    public bool ValidateHit(Hit hit) => true;
    public void RespondToHit(Hit hit) {}

    protected void Update() => PerformUpdate(StateMachine.RunUpdate);

    public void EventOnly_OnPossessionDetection(Collider other) {
        Entity entity = other.GetComponentInParent<Entity>();
        OnPossessorDetected?.Invoke(this, StateMachine, entity);
    }
}

}
