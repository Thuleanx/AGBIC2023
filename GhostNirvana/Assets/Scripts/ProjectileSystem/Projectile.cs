using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Base;
using Optimization;
using CombatSystem;

namespace Danmaku {

public class Projectile : PoolableEntity, IHitResponder {
    Entity owner;

    new Rigidbody rigidbody;
    Entity IHitResponder.Owner => owner;

    [SerializeField] float damage;
    [SerializeField] Hitbox hitbox;

    void Awake() {
        rigidbody = GetComponent<Rigidbody>();
        hitbox = GetComponentInChildren<Hitbox>();
        hitbox.HitResponder = this;
    }

    public void Initialize(Vector3 velocity, bool faceDirection = true) {
        rigidbody.velocity = velocity;
    }

    bool IHitResponder.ValidateHit(Hit hit) {
        return true;
    }

    void IHitResponder.RespondToHit(Hit hit) {
        Entity targetOwner = hit.Hurtbox.HurtResponder.Owner;
        (targetOwner as IHurtable)?.TakeDamage(damage, null, hit);

		if (this.gameObject.activeInHierarchy) 
			this.Dispose();
    }

    void Update() {
        hitbox.CheckForHits();
    }
}

}
