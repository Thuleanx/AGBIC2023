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
    [SerializeField] StatusRuntimeSet allAppliances;
    [BoxGroup("Movement"), Range(0, 720), SerializeField] float turnSpeed = 100;
	[SerializeField] GameObject onDeathVFX;

    [BoxGroup("Combat")] float summonRange;
    [BoxGroup("Combat")] BaseStats summonBaseStats;

    public UnityEvent<Ghosty> OnPossessionInterupt = new UnityEvent<Ghosty>();
    public BigGhostyStateMachine StateMachine {get; private set; }

    protected override void Awake() {
        base.Awake();
        StateMachine = GetComponent<BigGhostyStateMachine>();
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

    protected void Update() => PerformUpdate(StateMachine.RunUpdate);

    UnityEvent<BigGhosty> onSummonComplete;
    public void AnimationOnly_SummonAnimationComplete() {
        onSummonComplete?.Invoke(this);
    }

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
}

}
