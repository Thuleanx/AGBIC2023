using Base;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CombatSystem {

public interface IHitbox {
    public IHitResponder HitResponder { get; set;}

    public void CheckForHits();
}

public interface IHitResponder {
    public Entity Owner { get; }
    public bool ValidateHit(Hit hit);
    public void RespondToHit(Hit hit);
}

public interface IHurtbox {
    public bool Active { get; }
    public IHurtResponder HurtResponder { get; set; }

    public bool ValidateHit(Hit hit);
}

public interface IHurtResponder {
    public Entity Owner { get; }

    public bool ValidateHit(Hit hit);
    public void RespondToHit(Hit hit);
}

public interface IHurtable {
    public void TakeDamage(float damageAmount, DamageType damageType, Hit hit)
        => OnTakeDamage(damageAmount, damageType, hit);

    protected void OnTakeDamage(float damageAmount,
                                DamageType damageType,
                                Hit hit);
}

}
