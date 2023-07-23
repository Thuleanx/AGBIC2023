using System.Collections.Generic;
using UnityEngine;
using CombatSystem;
using Base;

namespace GhostNirvana {

public abstract class Enemy<Input> 
    : PossessableAgent<Input>, IHurtable, IHurtResponder, IHitResponder {

    public Status Status { get; private set; }

    public Entity Owner => this;

#region Inherited members
    public void RespondToHurt(Hit hit) {
    }

    public bool ValidateHit(Hit hit) => true;

    void IHurtable.OnTakeDamage(float damageAmount, DamageType damageType, Hit hit)
        => Status.TakeDamage(damageAmount);
#endregion

    List<Hitbox> hitboxes = new List<Hitbox>();

    protected override void Awake() {
        base.Awake();
        Status = GetComponent<Status>();
        Status.Owner = this;
        hitboxes.AddRange(GetComponentsInChildren<Hitbox>());
    }

    protected void CheckForHits() {
        foreach (Hitbox hitbox in hitboxes)
            hitbox.CheckForHits();
    }

    public void RespondToHit(Hit hit) {
        Entity targetOwner = hit.Hurtbox.HurtResponder.Owner;
        (targetOwner as IHurtable)?.TakeDamage(Status.BaseStats.Damage, null, hit);
        (targetOwner as IKnockbackable)?.ApplyKnockback(Status.BaseStats.Knockback, hit.Normal, hit);
    }
}

}
