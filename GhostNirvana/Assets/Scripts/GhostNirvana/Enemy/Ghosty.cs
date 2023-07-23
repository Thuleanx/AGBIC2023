using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using Optimization;
using Control;
using Danmaku;
using CombatSystem;
using Utils;
using Base;

namespace GhostNirvana {

[RequireComponent(typeof(CharacterController))]
public partial class Ghosty : PossessableAgent<Ghosty.Input>, IHurtable, IHurtResponder, IHitResponder {

    Entity IHurtResponder.Owner => this;
    public Entity Owner => this;

    public enum States {
        Seek,
        Death
    }

    [SerializeField] ExperienceGem droppedExperienceGem;

    #region Components
    [field:SerializeField]
    public Status Status {
        get;
        private set;
    }

    #endregion

    public struct Input {
        public Vector3 desiredMovement;
    }

    List<Hitbox> hitboxes = new List<Hitbox>();

    protected override void Awake() {
        base.Awake();
		Status = GetComponent<Status>();
        Status.Owner = this;

        hitboxes.AddRange(GetComponentsInChildren<Hitbox>());
    }

    protected void OnEnable() {
        IHurtResponder.ConnectChildrenHurtboxes(this);
        IHitResponder.ConnectChildrenHitboxes(this);
        Status.OnDeath.AddListener(OnDeath);
        if (HealthBarManager.Instance)
            HealthBarManager.Instance.AddStatus(Status);
        Status.HealToFull();
    }

    protected void OnDisable() {
        IHurtResponder.DisconnectChildrenHurtboxes(this);
        IHitResponder.ConnectChildrenHitboxes(this);
        HealthBarManager.Instance.RemoveStatus(Status);
        Status.OnDeath.RemoveListener(OnDeath);
    }

    protected void Start() {
        if (!HealthBarManager.Instance.IsTrackingStatus(Status))
            HealthBarManager.Instance.AddStatus(Status);
    }

    protected void Update() { 
        PerformUpdate(NormalUpdate);
    }

    void NormalUpdate() {
        Vector3 desiredVelocity = input.desiredMovement * Status.BaseStats.MovementSpeed;

        Velocity = Mathx.Damp(Vector3.Lerp, Velocity, desiredVelocity,
                              (Velocity.sqrMagnitude > desiredVelocity.sqrMagnitude)
                              ? Status.BaseStats.DeccelerationAlpha : Status.BaseStats.AccelerationAlpha,
                              Time.deltaTime);

        if (!Miyu.Instance) return;

        // TODO: rid of magic number
        float hitboxCheckingDistance = 2;
        bool closeToPlayer = (Miyu.Instance.transform.position - transform.position).sqrMagnitude < hitboxCheckingDistance;
        if (closeToPlayer) foreach (Hitbox hitbox in hitboxes)
            hitbox.CheckForHits();
    }

    void IHurtable.OnTakeDamage(float damageAmount, DamageType damageType, Hit hit)
        => Status.TakeDamage(damageAmount);


    void IHurtResponder.RespondToHit(Hit hit) {
    }

    void OnDeath(Status status) {
        // spawn experience gem
        ObjectPoolManager.Instance?.Borrow(gameObject.scene, droppedExperienceGem, transform.position);
        Dispose();
    }

    protected override IEnumerator IDispose() {
        // actually dispose the thing
        yield return base.IDispose();
    }

    bool IHurtResponder.ValidateHit(Hit hit) => true;
    public bool ValidateHit(Hit hit) => true;

    public void RespondToHit(Hit hit) {
        Entity targetOwner = hit.Hurtbox.HurtResponder.Owner;
        (targetOwner as IHurtable)?.TakeDamage(Status.BaseStats.Damage, null, hit);
    }
}

}
