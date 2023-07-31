using Base;
using UnityEngine;
using UnityEngine.Events;
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

    public static void ConnectChildrenHitboxes<T>(T responder) where T : MonoBehaviour, IHitResponder {
        foreach (IHitbox hitbox in responder.GetComponentsInChildren<IHitbox>())
            hitbox.HitResponder = responder;
    }

    public static void DisconnectChildrenHitboxes<T>(T responder) where T : MonoBehaviour, IHitResponder {
        foreach (IHitbox hitbox in responder.GetComponentsInChildren<IHitbox>())
            if (hitbox.HitResponder == responder)
                hitbox.HitResponder = null;
    }
}

public interface IHurtbox {
    public bool Active { get; }
    public IHurtResponder HurtResponder { get; set; }

    public bool ValidateHit(Hit hit);
}

public interface IHurtResponder {
    public Entity Owner { get; }

    public bool ValidateHit(Hit hit);
    public void RespondToHurt(Hit hit);

    public static void ConnectChildrenHurtboxes<T>(T responder) where T : MonoBehaviour, IHurtResponder {
        foreach (IHurtbox hurtbox in responder.GetComponentsInChildren<IHurtbox>())
            hurtbox.HurtResponder = responder;
    }

    public static void DisconnectChildrenHurtboxes<T>(T responder) where T : MonoBehaviour, IHurtResponder {
        foreach (IHurtbox hurtbox in responder.GetComponentsInChildren<IHurtbox>())
            if (hurtbox.HurtResponder == responder)
                hurtbox.HurtResponder = null;
    }
}

public interface IHurtable {
    public UnityEvent<IHurtable, float, DamageType> OnDamage {get;}
    public void TakeDamage(float damageAmount, DamageType damageType, Hit hit) {
        OnTakeDamage(damageAmount, damageType, hit);
        OnDamage?.Invoke(this, damageAmount, damageType);
    }

    protected void OnTakeDamage(float damageAmount,
                                DamageType damageType,
                                Hit hit);
}

public interface IKnockbackable {
    public void ApplyKnockback(float amount, Vector3 dir) => OnKnockback(amount, dir);

    protected void OnKnockback(float amount, Vector3 dir);
}

}
