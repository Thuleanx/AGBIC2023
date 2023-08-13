using System.Collections.Generic;
using UnityEngine;
using CombatSystem;
using Base;
using NaughtyAttributes;
using UnityEngine.Events;

namespace GhostNirvana {

public abstract class Enemy<Input>
    : PossessableAgent<Input>, IHurtable, IHurtResponder, IHitResponder {

    public Entity Owner {
		get => this; 
		set {
			Debug.LogError("Can't set owner of an enemy.");
		}
	}

    public Status Status { get; private set; }

    [SerializeField] UnityEvent<IHurtable, int, DamageType, Hit> _OnDamage;
    public UnityEvent<IHurtable, int, DamageType, Hit> OnBeforeDamage => _OnDamage;

    #region Inherited members
    public void RespondToHurt(Hit hit) {
    }

    public bool ValidateHit(Hit hit) => true;

    void IHurtable.OnTakeDamage(int damageAmount, DamageType damageType, Hit hit)
        => Status.TakeDamage(damageAmount);
#endregion

    List<Hitbox> hitboxes = new List<Hitbox>();
    [SerializeField, Required] protected MovableAgentRuntimeSet allEnemies;
    [SerializeField] HurtableRuntimeSet allHurtables;

    protected override void Awake() {
        base.Awake();
        Status = GetComponent<Status>();
        Status.Owner = this;
        hitboxes.AddRange(GetComponentsInChildren<Hitbox>());
    }

    protected override void OnEnable() {
		base.OnEnable();
        allEnemies.Add(this);
        allHurtables.Add(this);
    }
    protected virtual void OnDisable() {
        allEnemies.Remove(this);
        allHurtables.Remove(this);
    }

    protected void CheckForHits() {
        foreach (Hitbox hitbox in hitboxes)
            hitbox.CheckForHits();
    }

    public void RespondToHit(Hit hit) {
        Entity targetOwner = hit.Hurtbox.HurtResponder.Owner;
        (targetOwner as IHurtable)?.TakeDamage(Status.BaseStats.Damage, null, hit);
        (targetOwner as IKnockbackable)?.ApplyKnockback(Status.BaseStats.Knockback, hit.Normal);
    }
}


}
