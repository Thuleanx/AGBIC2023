using UnityEngine;

using Base;
using Optimization;
using CombatSystem;
using NaughtyAttributes;
using System.Collections;

namespace Danmaku {

public class Projectile : PoolableEntity, IHitResponder {
    Entity owner;

    new Rigidbody rigidbody;
    Entity IHitResponder.Owner {get => owner; set => owner = value; }

    [SerializeField, ReadOnly] int damage;
    [SerializeField, ReadOnly] float knockback;
    [SerializeField, ReadOnly] int pierce;
    [SerializeField, ReadOnly] Hitbox hitbox;
    [SerializeField, ShowAssetPreview] GameObject onHitEffect;

    void Awake() {
        rigidbody = GetComponent<Rigidbody>();
        hitbox = GetComponentInChildren<Hitbox>();
        hitbox.HitResponder = this;
    }

    public void Initialize(int damage, float knockback, int pierce, Vector3 velocity, bool faceDirection = true) {
        rigidbody.velocity = velocity;
        this.damage = damage;
        this.knockback = knockback;
        this.pierce = pierce;
        if (faceDirection && velocity.sqrMagnitude > 0) transform.rotation = Quaternion.LookRotation(forward: velocity, upwards: Vector3.up);
    }

    bool IHitResponder.ValidateHit(Hit hit) {
        return true;
    }

    void IHitResponder.RespondToHit(Hit hit) {
        if (!this.gameObject.activeInHierarchy) return;

        Entity targetOwner = hit.Hurtbox.HurtResponder.Owner;
        if (targetOwner) {
            (targetOwner as IHurtable)?.TakeDamage(damage, null, hit);
            (targetOwner as IKnockbackable)?.ApplyKnockback(knockback, rigidbody.velocity.normalized);
        }

        (owner as IHitResponder)?.RespondToHit(hit);

        // if pierce count hit
		if (this.gameObject.activeInHierarchy && pierce--<=0) {
            SpawnOnHitEffect();
			this.Dispose();
        }
    }

    void Update() {
        hitbox.CheckForHits();
    }

    void OnCollisionEnter(Collision collision) {
        collision.collider.GetComponentInParent<RespondToBulletHit>()?.OnBulletHit?.Invoke();

		if (this.gameObject.activeInHierarchy) {
            SpawnOnHitEffect();
			this.Dispose();
        }
    }

    void SpawnOnHitEffect() {
        if (onHitEffect)
            ObjectPoolManager.Instance?.Borrow(App.GetActiveScene(),
                onHitEffect.transform, transform.position);
    }
}

}
