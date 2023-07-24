using UnityEngine;
using Optimization;
using CombatSystem;
using NaughtyAttributes;
using Utils;

namespace GhostNirvana {

[RequireComponent(typeof(CharacterController))]
public partial class Ghosty : Enemy<Ghosty.Input> {
    public enum States {
        Seek,
        Death
    }

    [SerializeField, ShowAssetPreview] Transform droppedExperienceGem;
    [SerializeField] StatusRuntimeSet allEnemyStatus;

    public struct Input {
        public Vector3 desiredMovement;
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

    protected void Update() => PerformUpdate(NormalUpdate);

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
        if (closeToPlayer) CheckForHits();
    }

    void OnDeath(Status status) {
        // spawn experience gem
        ObjectPoolManager.Instance?.Borrow(gameObject.scene, droppedExperienceGem, transform.position);
        Dispose();
    }
}

}
