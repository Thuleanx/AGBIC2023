using AI;
using Base;
using UnityEngine;
using UnityEngine.Events;
using CombatSystem;
using NaughtyAttributes;
using System.Collections.Generic;

namespace GhostNirvana {

[RequireComponent(typeof(Status))]
public partial class Appliance : 
    PossessableAgent<Appliance.Input>, IHurtable, IHurtResponder, IHitResponder {


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

    Entity IHurtResponder.Owner => this;
    public Entity Owner => this;


    UnityEvent<Appliance, StateMachine<Appliance, Appliance.States>, Entity> OnPossessorDetected = new UnityEvent<Appliance, StateMachine<Appliance, States>, Entity>();
    List<Hitbox> hitboxes = new List<Hitbox>();

    protected override void Awake() {
        base.Awake();
        StateMachine = GetComponent<ApplianceStateMachine>();
        Status = GetComponent<Status>();
        Status.Owner = this;
        hitboxes.AddRange(GetComponentsInChildren<Hitbox>());
    }

    protected void OnEnable() {
        IHurtResponder.ConnectChildrenHurtboxes(this);
        IHitResponder.ConnectChildrenHitboxes(this);
    }

    protected void OnDisable() {
        IHurtResponder.DisconnectChildrenHurtboxes(this);
        IHitResponder.ConnectChildrenHitboxes(this);
    }

    void IHurtable.OnTakeDamage(float damageAmount, DamageType damageType, Hit hit)
        => Status.TakeDamage(damageAmount);

    public bool ValidateHit(Hit hit) => true;
    public void RespondToHit(Hit hit) {
        Entity targetOwner = hit.Hurtbox.HurtResponder.Owner;
        (targetOwner as IHurtable)?.TakeDamage(Status.BaseStats.Damage, null, hit);
    }

    protected void Update() => PerformUpdate(StateMachine.RunUpdate);

    public void EventOnly_OnPossessionDetection(Collider other) {
        Entity entity = other.GetComponentInParent<Entity>();
        OnPossessorDetected?.Invoke(this, StateMachine, entity);
    }
}

}
