using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Base;
using Optimization;
using CombatSystem;
using NaughtyAttributes;

namespace CombatSystem {

public class GenericHitResponder : PoolableEntity, IHitResponder {
    Entity owner;

    Entity IHitResponder.Owner { get => owner; set => owner = value; }

    [SerializeField] int damage;
    [SerializeField] float knockback;
    [SerializeField, ReadOnly] Hitbox hitbox;

    void Awake() {
        hitbox = GetComponentInChildren<Hitbox>();
        hitbox.HitResponder = this;
    }

    protected override void OnEnable() {
        base.OnEnable();
        IHitResponder.ConnectChildrenHitboxes(this);
    }

    protected void OnDisable() {
        IHitResponder.DisconnectChildrenHitboxes(this);
    }

    bool IHitResponder.ValidateHit(Hit hit) {
        return true;
    }

    void IHitResponder.RespondToHit(Hit hit) {
        if (!this.gameObject.activeInHierarchy) return;

        Entity targetOwner = hit.Hurtbox.HurtResponder.Owner;
        (targetOwner as IHurtable)?.TakeDamage(damage, null, hit);
        (targetOwner as IKnockbackable)?.ApplyKnockback(knockback, hit.Normal);
    }

    public void Initialize(int damage, float knockback) {
        this.damage = damage;
        this.knockback = knockback;
    }

    void Update() {
        hitbox.CheckForHits();
    }
}

}
