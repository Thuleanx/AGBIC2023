using UnityEngine;
using Base;
using Optimization;
using CombatSystem;
using NaughtyAttributes;

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
        this.speed = velocity.magnitude;
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
                Vector3 normal = collision.contacts[0].normal;

                if (flatBounces) currentVelocity.y = normal.y = 0;

                currentVelocity.Normalize();
                normal.Normalize();

                Vector3 reflect = currentVelocity - 2 * Vector3.Dot(currentVelocity, normal) * normal;
                reflect.Normalize();

                rigidbody.velocity = reflect * speed;
                velocity = reflect * speed;
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
