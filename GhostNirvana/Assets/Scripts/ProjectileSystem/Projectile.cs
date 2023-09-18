using UnityEngine;
using Base;
using Optimization;
using CombatSystem;
using NaughtyAttributes;
using ScriptableBehaviour;
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
    [SerializeField, ReadOnly] int ricochet;
    [SerializeField, ReadOnly] float ricochetRange;
    [SerializeField, ReadOnly] Hitbox hitbox;
    [SerializeField, ShowAssetPreview] GameObject onHitEffect;
    [SerializeField] GameObjectRuntimeSet allEnemiesObject;
	[SerializeField] bool shouldPlayEffectOnHit;
    Vector3 velocity;
    float speed;
    bool faceDirection;

    void Awake() {
        rigidbody = GetComponent<Rigidbody>();
        hitbox = GetComponentInChildren<Hitbox>();
        hitbox.HitResponder = this;
    }

    public void Initialize(int damage, float knockback, int pierce, int bounce, int ricochet, float ricochetRange, Vector3 velocity, bool faceDirection = true) {
        rigidbody.velocity = this.velocity = velocity;
        this.speed = velocity.magnitude;
        this.damage = damage;
        this.knockback = knockback;
        this.pierce = pierce;
        this.bounce = bounce;
        this.ricochet = ricochet;
        this.faceDirection = faceDirection;
        this.ricochetRange = ricochetRange;
        if (faceDirection) RotateToFaceVelocity();
    }

    void RotateToFaceVelocity() {
        if (velocity.sqrMagnitude > 0) transform.rotation = Quaternion.LookRotation(forward: velocity, upwards: Vector3.up);
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
		if (this.gameObject.activeInHierarchy)
			HandleHitboxHit(hit);
    }

	void HandleHitboxHit(Hit hit) {
		if (ricochet > 0) {
			Vector3? closestEnemyDir = FindClosestEnemyDirection(hit);
			if (closestEnemyDir.HasValue) {
				SpawnOnHitEffect(hit.Position);
				Redirect(closestEnemyDir.Value.normalized);
				ricochet--;
				return;
			}
		}
		if (pierce--<=0) this.Dispose();
		if (shouldPlayEffectOnHit) SpawnOnHitEffect(hit.Position);
	}

    Vector3? FindClosestEnemyDirection(Hit hit) {
        // redirect to closest enemy
        Vector3? closestEnemyDir = null;
        float bestDotSquared = 1;

        GameObject enemyHit = (hit.Hurtbox.HurtResponder as MonoBehaviour)?.gameObject;

        if (allEnemiesObject)
        foreach (GameObject enemy in allEnemiesObject) {
            if (enemy == enemyHit) continue;

            Vector3 displacement = enemy.transform.position - transform.position;
            displacement.y = 0;

            float squaredDisplacementDistance = displacement.sqrMagnitude;
            if (squaredDisplacementDistance >= ricochetRange * ricochetRange) continue;

            float displacementProjectedOntoVelocitySquared = Vector3.Dot(displacement, velocity);
            displacementProjectedOntoVelocitySquared *= displacementProjectedOntoVelocitySquared;

            if (!closestEnemyDir.HasValue || bestDotSquared * squaredDisplacementDistance < displacementProjectedOntoVelocitySquared * closestEnemyDir.Value.sqrMagnitude) {
                closestEnemyDir = displacement;
                bestDotSquared = displacementProjectedOntoVelocitySquared;
            }
        }

        return closestEnemyDir;
    }

    protected override IEnumerator IDispose() {
        SpawnOnHitEffect(null);
        return base.IDispose();
    }

    void Redirect(Vector3 direction) {
        rigidbody.velocity = direction * speed;
        velocity = direction * speed;
        if (faceDirection) RotateToFaceVelocity();
    }

    void Update() {
        hitbox.CheckForHits();
    }

    void OnCollisionEnter(Collision collision) {
        collision.collider.GetComponentInParent<RespondToBulletHit>()?.OnBulletHit?.Invoke();

		if (this.gameObject.activeInHierarchy) {
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

				SpawnOnHitEffect(null);
                Redirect(reflect);
            }
        }
    }

    void SpawnOnHitEffect(Vector3? pos) {
        if (onHitEffect)
            ObjectPoolManager.Instance?.Borrow(App.GetActiveScene(),
                onHitEffect.transform, pos ?? transform.position);
    }
}

}
