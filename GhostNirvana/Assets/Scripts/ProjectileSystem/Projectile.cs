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

    [SerializeField] bool flatBounces = true;
    [SerializeField, ReadOnly] int damage;
    [SerializeField, ReadOnly] float knockback;
    [SerializeField, ReadOnly] int pierce;
    [SerializeField, ReadOnly] int bounce;
    [SerializeField, ReadOnly] Hitbox hitbox;
    [SerializeField, ShowAssetPreview] GameObject onHitEffect;
    Vector3 velocity;
    float speed;

    void Awake() {
        rigidbody = GetComponent<Rigidbody>();
        hitbox = GetComponentInChildren<Hitbox>();
        hitbox.HitResponder = this;
    }

    public void Initialize(int damage, float knockback, int pierce, int bounce, Vector3 velocity, bool faceDirection = true) {
        rigidbody.velocity = this.velocity = velocity;
        speed = velocity.magnitude;
        this.damage = damage;
        this.knockback = knockback;
        this.pierce = pierce;
        this.bounce = bounce;
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
            if (bounce--<=0)
			    this.Dispose();
            else {
                // bounce
                Vector3 currentVelocity = velocity; 
                Vector3 normal = collision.contacts[0].normal.normalized;

                /* Vector3 velocity = flatBounces ? Vector3.ProjectOnPlane(rigidbody.velocity, planeNormal: Vector3.up) : rigidbody.velocity; */
                currentVelocity.Normalize();
                /* Vector3 normal = flatBounces ? Vector3.ProjectOnPlane(collision.contacts[0].normal, planeNormal: Vector3.up) : collision.contacts[0].normal; */
                normal.Normalize();

                Vector3 reflect = 2 * Vector3.Dot(currentVelocity, normal) * currentVelocity - currentVelocity;

                rigidbody.velocity = reflect * speed;
                currentVelocity = rigidbody.velocity;
            }
        }
    }

    void SpawnOnHitEffect() {
        if (onHitEffect)
            ObjectPoolManager.Instance?.Borrow(App.GetActiveScene(),
                onHitEffect.transform, transform.position);
    }
}

}
