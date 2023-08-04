using System;
using UnityEngine;
using UnityEngine.Events;
using Optimization;
using CombatSystem;
using NaughtyAttributes;
using Utils;

namespace GhostNirvana {

[RequireComponent(typeof(CharacterController))]
public partial class Ghosty : Enemy<Ghosty.Input> {
    public enum States {
        Seek,
        Possessing,
        Possessed,
        Death
    }

    [SerializeField, ShowAssetPreview] Transform droppedExperienceGem;
    [SerializeField] StatusRuntimeSet allEnemyStatus;
    [BoxGroup("Movement"), Range(0, 720), SerializeField] float turnSpeed = 100;
    [BoxGroup("Combat"), SerializeField] float possessionCooldownSeconds = 4.0f;

    UnityEvent<Ghosty, Appliance> OnPossession = new UnityEvent<Ghosty, Appliance>();
    UnityEvent<Ghosty> OnPossessionFinish = new UnityEvent<Ghosty>();

    Timer possessionCooldownActive;

    public GhostyStateMachine StateMachine {get; private set; }
    public bool IsPossessing => StateMachine ? StateMachine.State == States.Possessing : false;
    public bool CanPossess => !IsPossessing && !possessionCooldownActive;

    public struct Input {
        public Vector3 desiredMovement;
    }

    protected override void Awake() {
        base.Awake();
        StateMachine = GetComponent<GhostyStateMachine>();
    }

    protected override void OnEnable() {
        base.OnEnable();
        IHurtResponder.ConnectChildrenHurtboxes(this);
        IHitResponder.ConnectChildrenHitboxes(this);
        Status.OnDeath.AddListener(OnDeath);
        allEnemyStatus.Add(Status);
        Status.HealToFull();

        possessionCooldownActive = 0.0f;
    }


    protected override void OnDisable() {
        base.OnDisable();
        IHurtResponder.DisconnectChildrenHurtboxes(this);
        IHitResponder.ConnectChildrenHitboxes(this);
        allEnemyStatus.Remove(Status);
        Status.OnDeath.RemoveListener(OnDeath);
    }

    protected void Update() => PerformUpdate(StateMachine.RunUpdate);

    public void ApplianceOnly_Possess(Appliance appliance) {
        if (IsPossessing) return;
        OnPossession?.Invoke(this, appliance);
    }

    void OnDeath(Status status) {
        // spawn experience gem
        ObjectPoolManager.Instance?.Borrow(gameObject.scene, droppedExperienceGem, transform.position);

        if (IsPossessing) {
            // need to release the appliance
            Appliance appliance = StateMachine.Blackboard["possessingAppliance"] as Appliance;
            appliance.OnPossessionInterupt?.Invoke(appliance);
        }

        StateMachine.SetState(States.Death);
    }

    public void AnimationOnly_PossessionFinish() {
        if (!IsPossessing) return;
        OnPossessionFinish?.Invoke(this);
    }
}

}
