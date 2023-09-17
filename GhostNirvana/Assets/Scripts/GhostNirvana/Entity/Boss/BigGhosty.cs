using Base;
using System;
using UnityEngine;
using UnityEngine.Events;
using Optimization;
using CombatSystem;
using NaughtyAttributes;
using Utils;
using Danmaku;

namespace GhostNirvana {

[RequireComponent(typeof(CharacterController))]
public partial class BigGhosty : Enemy<StandardMovementInput> {
    [System.Serializable]
    public enum States {
        Seek,
        Summon,
        LaunchAttack,
        Death
    }

    [SerializeField, ShowAssetPreview] GameObject ghostyGoon;
    [SerializeField] StatusRuntimeSet allEnemyStatus;
    [SerializeField] MovableAgentRuntimeSet allAppliances;
    [BoxGroup("Movement"), Range(0, 720), SerializeField] float turnSpeed = 100;

	[SerializeField, BoxGroup("Combat"), ShowAssetPreview] GameObject onDeathVFX;
    [SerializeField, BoxGroup("Combat"), ShowAssetPreview] GameObject attackProjectile;
    [SerializeField, BoxGroup("Combat"), Range(0, 40f)] float waveSpeed;
    [SerializeField, BoxGroup("Combat")] float summonRange;
    [SerializeField, BoxGroup("Combat")] BaseStats summonBaseStats;
    [SerializeField, BoxGroup("Combat"), Range(0, 720)] float turnSpeedWhileAttacking = 100;
    [SerializeField, BoxGroup("Combat"), Range(0, 720)] float turnSpeedWhileSummoning = 100;
    [SerializeField, BoxGroup("Combat"), MinMaxSlider(0, 100)] Vector2 summonCooldownSeconds;
    [SerializeField, BoxGroup("Combat"), MinMaxSlider(0, 100)] Vector2 attackCooldownSeconds;
    [SerializeField, BoxGroup("Combat")] AnimationCurve hasteScalingByHealth;
    [SerializeField, BoxGroup("Combat")] float accelerationDoublingRange;

    float healthFraction => (float) Status.Health / Status.BaseStats.MaxHealth;

    Vector2 currentSummonCooldown => summonCooldownSeconds / currentHaste;
    Vector2 currentAttackCooldown => attackCooldownSeconds / currentHaste;
    float currentHaste => hasteScalingByHealth.Evaluate(healthFraction);

    float currentMaxSpeed => currentHaste * Status.BaseStats.MovementSpeed;
    float currentAcceleration => currentHaste * Status.BaseStats.Acceleration * (isCloseToPlayer ? 2 : 1);
    bool isCloseToPlayer => Miyu.Instance ?
        (Miyu.Instance.transform.position - transform.position).sqrMagnitude
        <= accelerationDoublingRange * accelerationDoublingRange : false;

    public UnityEvent<Ghosty> OnPossessionInterupt = new UnityEvent<Ghosty>();
    public BigGhostyStateMachine StateMachine {get; private set; }
    public Animator Anim {get; private set; }

    float canSummonUpdatesPerSecond = 4;
    float lastCanSummonUpdate;
    bool canSummon = false;

    Timer summonCooldown, attackCooldown;

    protected override void Awake() {
        base.Awake();
        StateMachine = GetComponent<BigGhostyStateMachine>();
        Anim = GetComponentInChildren<Animator>();
    }

    protected override void OnEnable() {
        base.OnEnable();
        IHurtResponder.ConnectChildrenHurtboxes(this);
        IHitResponder.ConnectChildrenHitboxes(this);
        Status.OnDeath.AddListener(OnDeath);
        allEnemyStatus.Add(Status);
        Status.HealToFull();
    }

    protected override void OnDisable() {
        base.OnDisable();
        IHurtResponder.DisconnectChildrenHurtboxes(this);
        IHitResponder.ConnectChildrenHitboxes(this);
        allEnemyStatus.Remove(Status);
        Status.OnDeath.RemoveListener(OnDeath);
    }

    protected void Update() {
        PerformUpdate(StateMachine.RunUpdate);
        bool shouldUpdateCanSummon = (Time.time - lastCanSummonUpdate) * canSummonUpdatesPerSecond >= 1;
        if (shouldUpdateCanSummon) {
            canSummon = GetCanSummon();
            lastCanSummonUpdate = Time.time;
        }
        Anim?.SetFloat("Haste", currentHaste);
    }

    [SerializeField] UnityEvent<BigGhosty> onSummonChanneled;
    [SerializeField] UnityEvent<BigGhosty> onSummonComplete;
    public void AnimationOnly_SummonAnimationChanneled() => onSummonChanneled?.Invoke(this);
    public void AnimationOnly_SummonAnimationComplete() => onSummonComplete?.Invoke(this);

    [SerializeField] UnityEvent<BigGhosty> onAttackChanneled;
    [SerializeField] UnityEvent<BigGhosty> onAttackComplete;
    public void AnimationOnly_AttackAnimationChanneled() => onAttackChanneled?.Invoke(this);
    public void AnimationOnly_AttackAnimationComplete() => onAttackComplete?.Invoke(this);

    void OnDeath(Status status) {
        StateMachine.SetState(States.Death);
    }

    void SummonGhostyAt(Vector3 position) {
        PoolableEntity possessingGhost = ObjectPoolManager.Instance.Borrow(
            App.GetActiveScene(),
            ghostyGoon.GetComponent<PoolableEntity>(),
            position
        );

        BaseStatsMonoBehaviour baseStatsHolder = possessingGhost.GetComponent<BaseStatsMonoBehaviour>();
        baseStatsHolder.Stats = summonBaseStats;

        possessingGhost.GetComponent<Status>().HealToFull();
        possessingGhost.GetComponentInChildren<Animator>()?.SetTrigger("Summon");
    }

    bool GetCanSummon() {
        bool hasAgentInRange = false;
        foreach (MovableAgent applianceAgent in allAppliances) {
            Vector3 displacement = applianceAgent.transform.position - transform.position;
            hasAgentInRange |= displacement.sqrMagnitude <= summonRange;
        }
        return hasAgentInRange;
    }

    void AttackAt(Vector3 direction) {
        Projectile projectile = attackProjectile.GetComponent<Projectile>();
        Projectile projectileSpawned = ObjectPoolManager.Instance.Borrow(
            App.GetActiveScene(),
            projectile,
            transform.position
        );

        Vector3 bulletVelocity = direction * waveSpeed;
        projectileSpawned.Initialize(
            damage: 1,
            knockback: 8,
            pierce: 1,
            bounce: 0,
            ricochet: 0,
            ricochetRange: 0,
            velocity: bulletVelocity,
            faceDirection: true
        );
    }
}

}
