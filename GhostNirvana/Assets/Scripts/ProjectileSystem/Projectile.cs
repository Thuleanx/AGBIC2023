using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Base;
using Optimization;
using CombatSystem;
using NaughtyAttributes;

namespace Danmaku {

public class Projectile : PoolableEntity, IHitResponder {
    Entity owner;

    new Rigidbody rigidbody;
    Entity IHitResponder.Owner => owner;

    [SerializeField, ReadOnly] float damage;
    [SerializeField, ReadOnly] float knockback;
    [SerializeField, ReadOnly] Hitbox hitbox;

    void Awake() {
        rigidbody = GetComponent<Rigidbody>();
        hitbox = GetComponentInChildren<Hitbox>();
        hitbox.HitResponder = this;
    }

    public void Initialize(float damage, float knockback, Vector3 velocity, bool faceDirection = true) {
        rigidbody.velocity = velocity;
        this.damage = damage;
        this.knockback = knockback;
    }

    bool IHitResponder.ValidateHit(Hit hit) {
        return true;
    }

    void IHitResponder.RespondToHit(Hit hit) {
        Entity targetOwner = hit.Hurtbox.HurtResponder.Owner;
        (targetOwner as IHurtable)?.TakeDamage(damage, null, hit);
        (targetOwner as IKnockbackable)?.ApplyKnockback(knockback, rigidbody.velocity.normalized);

		if (this.gameObject.activeInHierarchy) 
			this.Dispose();
    }

    void Update() {
        hitbox.CheckForHits();
    }
}

}
