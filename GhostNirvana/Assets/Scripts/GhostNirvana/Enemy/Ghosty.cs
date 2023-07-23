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
public partial class Ghosty : PossessableAgent<Ghosty.Input>, IHurtable, IHurtResponder {

    Entity IHurtResponder.Owner => this;

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

    protected override void Awake() {
        base.Awake();
		Status = GetComponent<Status>();
        Status.Owner = this;
    }

    protected void OnEnable() {
        Status.OnDeath.AddListener(OnDeath);
        foreach (Hurtbox hurtbox in GetComponentsInChildren<Hurtbox>())
            hurtbox.HurtResponder = this;
        if (HealthBarManager.Instance)
            HealthBarManager.Instance.AddStatus(Status);
        Status.HealToFull();
    }

    protected void Start() {
        if (!HealthBarManager.Instance.IsTrackingStatus(Status))
            HealthBarManager.Instance.AddStatus(Status);
    }

    protected void OnDisable() {
        HealthBarManager.Instance.RemoveStatus(Status);
    }

    protected void Update() {
        PerformUpdate(AdjustVelocity);
    }

    void AdjustVelocity() {
        Vector3 desiredVelocity = input.desiredMovement * Status.BaseStats.MovementSpeed;

        Velocity = Mathx.Damp(Vector3.Lerp, Velocity, desiredVelocity,
                              (Velocity.sqrMagnitude > desiredVelocity.sqrMagnitude)
                              ? Status.BaseStats.DeccelerationAlpha : Status.BaseStats.AccelerationAlpha,
                              Time.deltaTime);
    }

    void IHurtable.OnTakeDamage(float damageAmount, DamageType damageType, Hit hit)
        => Status.TakeDamage(damageAmount);

    bool IHurtResponder.ValidateHit(Hit hit) => true;

    void IHurtResponder.RespondToHit(Hit hit) {
    }
    void OnDeath(Status status) {
        Status.OnDeath.RemoveListener(OnDeath);
        // spawn experience gem
        ObjectPoolManager.Instance?.Borrow(gameObject.scene, droppedExperienceGem, transform.position);
        Dispose();
    }

    protected override IEnumerator IDispose() {
        // actually dispose the thing
        yield return base.IDispose();
    }
}

}
