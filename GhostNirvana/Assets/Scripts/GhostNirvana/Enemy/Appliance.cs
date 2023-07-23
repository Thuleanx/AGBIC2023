using AI;
using Base;
using UnityEngine;
using UnityEngine.Events;
using CombatSystem;
using NaughtyAttributes;
using System.Collections.Generic;

namespace GhostNirvana {

[RequireComponent(typeof(Status))]
public partial class Appliance : Enemy<Appliance.Input> {

    public enum States {
        Idle,
        Possessed
    }

    public struct Input {
        public Vector3 desiredMovement;
    };

    #region Components
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
    }

    protected void OnEnable() {
        IHurtResponder.ConnectChildrenHurtboxes(this);
        IHitResponder.ConnectChildrenHitboxes(this);
    }

    protected void OnDisable() {
        IHurtResponder.DisconnectChildrenHurtboxes(this);
        IHitResponder.ConnectChildrenHitboxes(this);
    }

    protected void Update() => PerformUpdate(StateMachine.RunUpdate);

    public void EventOnly_OnPossessionDetection(Collider other) {
        Entity entity = other.GetComponentInParent<Entity>();
        OnPossessorDetected?.Invoke(this, StateMachine, entity);
    }
}

}
