using Base;
using System;
using UnityEngine;
using UnityEngine.Events;
using Optimization;
using CombatSystem;
using NaughtyAttributes;
using Utils;

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

    [SerializeField, ShowAssetPreview] GameObject ghosty;
    [SerializeField] StatusRuntimeSet allEnemyStatus;
    [SerializeField] MovableAgentRuntimeSet allAppliances;
    [BoxGroup("Movement"), Range(0, 720), SerializeField] float turnSpeed = 100;

	[SerializeField, BoxGroup("Combat")] GameObject onDeathVFX;
    [SerializeField, BoxGroup("Combat")] float summonRange;
    [SerializeField, BoxGroup("Combat")] BaseStats summonBaseStats;
    [SerializeField, BoxGroup("Combat"), Range(0, 720)] float turnSpeedWhileAttacking = 100;
    [SerializeField, BoxGroup("Combat"), Range(0, 720)] float turnSpeedWhileSummoning = 100;
    [SerializeField, BoxGroup("Combat"), MinMaxSlider(0, 100)] Vector2 summonCooldownSeconds;
    [SerializeField, BoxGroup("Combat"), MinMaxSlider(0, 100)] Vector2 attackCooldownSeconds;

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
            ghosty.GetComponent<PoolableEntity>(),
            position
        );

        BaseStatsMonoBehaviour baseStatsHolder = possessingGhost.GetComponent<BaseStatsMonoBehaviour>();
        baseStatsHolder.Stats = summonBaseStats;

        possessingGhost.GetComponent<Status>().HealToFull();
    }

    bool GetCanSummon() {
        bool hasAgentInRange = false;
        foreach (MovableAgent applianceAgent in allAppliances) {
            Vector3 displacement = applianceAgent.transform.position - transform.position;
            hasAgentInRange |= displacement.sqrMagnitude <= summonRange;
        }
        return hasAgentInRange;
    }
}

}
