using Base;
using AI;
using UnityEngine;
using UnityEngine.Events;
using CombatSystem;
using NaughtyAttributes;
using Optimization;
using Utils;
using DG.Tweening;

namespace GhostNirvana {

[RequireComponent(typeof(Status))]
public partial class Appliance : Enemy<StandardMovementInput> {

    public enum States {
        Idle,
        BeforePossessed,
        Possessed,
        Collecting
    }

	[SerializeField] MovableAgentRuntimeSet allAppliances;

    #region Components
    [field:SerializeField, Required]
    public Collider PossessionDetection {get; private set; }

    public ApplianceStateMachine StateMachine {
        get; private set;
    }
    #endregion

    UnityEvent<Appliance, StateMachine<Appliance, Appliance.States>, Ghosty>
    OnPossessorDetected = new UnityEvent<Appliance, StateMachine<Appliance, States>, Ghosty>();

    [HideInInspector] public UnityEvent<Appliance> OnPossessionInterupt = new UnityEvent<Appliance>();
    [HideInInspector] public UnityEvent<Appliance> OnPossessionComplete = new UnityEvent<Appliance>();

    [SerializeField] StatusRuntimeSet allEnemyStatus;
    [field:SerializeField] public int Price {get; private set; }
    [BoxGroup("Movement"), Range(0, 720), SerializeField] float turnSpeed = 100;

    [SerializeField] float knockbackOnEjection = 100;
    [SerializeField] float cooldownAfterPossession = 10;
    [BoxGroup("Combat"), SerializeField, Required, ShowAssetPreview] GameObject AOEAttackPrefab;
    [BoxGroup("Combat"), SerializeField] Transform leftAttackAnchor;
    [BoxGroup("Combat"), SerializeField] Transform rightAttackAnchor;
    [BoxGroup("Combat"), SerializeField] GameObject possessionVFX;

    [BoxGroup("Collection"), SerializeField, ShowAssetPreview] GameObject clawHand;
    [BoxGroup("Collection"), SerializeField] float collectionPivotTop;
    [BoxGroup("Collection"), SerializeField] float aboveScreenDistance;
    [BoxGroup("Collection"), SerializeField] float floatDownDuration;
    [BoxGroup("Collection"), SerializeField] Ease floatDownEase;
    [BoxGroup("Collection"), SerializeField] float floatUpDuration;
    [BoxGroup("Collection"), SerializeField] Ease floatUpEase;
    [BoxGroup("Collection"), SerializeField] float stayDuration;
    [BoxGroup("Collection"), SerializeField] float delayTime = 0.5f;

    Timer possessionCooldown;

    public bool IsPossessedByGhost => StateMachine.State == States.Possessed;
    public bool IsBeingPossessed => StateMachine.State == States.BeforePossessed;
    public bool PossessionImmune => possessionCooldown;

    protected override void Awake() {
        base.Awake();
        StateMachine = GetComponent<ApplianceStateMachine>();
    }

    protected override void OnEnable() {
        base.OnEnable();
		allAppliances.Add(this);
        allEnemies.Remove(this);
        allEnemiesGameObject.Remove(gameObject);
        FreezePosition = true;
    }

    protected override void OnDisable() {
        base.OnDisable();
		allAppliances.Remove(this);
        IHurtResponder.DisconnectChildrenHurtboxes(this);
        IHitResponder.DisconnectChildrenHitboxes(this);
    }

    protected void Update() => PerformUpdate(StateMachine.RunUpdate);

    public void AOEAttack(bool isLeftAttack) {
        Transform attackAnchor = (isLeftAttack ? leftAttackAnchor : rightAttackAnchor);

        Transform aoeAttack = ObjectPoolManager.Instance.Borrow(App.GetActiveScene(),
            AOEAttackPrefab.transform, attackAnchor.position, attackAnchor.rotation);

        IHitResponder hitResponder = aoeAttack.GetComponent<IHitResponder>();
        hitResponder.Owner = this;
    }

    public void EventOnly_OnPossessionDetection(Collider other) {
        bool cannotBePossessed = (IsPossessedByGhost || IsBeingPossessed || possessionCooldown);
        if (cannotBePossessed) return;
        Ghosty possessor = other.GetComponentInParent<Ghosty>();
        if (!possessor || !possessor.CanPossess) return;
        OnPossessorDetected?.Invoke(this, StateMachine, possessor);
    }

    public void ApplianceCollectorOnly_Collect() => StateMachine.SetState(States.Collecting);

    public void DelayPossession(float time) {
        bool shouldChangePossessionCooldown = !possessionCooldown || possessionCooldown.TimeLeft < time;
        if (shouldChangePossessionCooldown)
            possessionCooldown = time;
    }
}

}
