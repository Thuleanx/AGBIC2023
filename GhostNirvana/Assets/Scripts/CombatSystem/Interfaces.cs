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
    public Entity Owner { get; set; }
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
    public UnityEvent<IHurtable, int, DamageType, Hit> OnBeforeDamage {get;}
    public void TakeDamage(int damageAmount, DamageType damageType, Hit hit) {
        OnBeforeDamage?.Invoke(this, damageAmount, damageType, hit);
        OnTakeDamage(damageAmount, damageType, hit);
    }

    protected void OnTakeDamage(int damageAmount,
                                DamageType damageType,
                                Hit hit);
}

public interface IKnockbackable {
    public void ApplyKnockback(float amount, Vector3 dir, bool ignoreKnockbackImmunity = false) => OnKnockback(amount, dir, ignoreKnockbackImmunity);
    protected void OnKnockback(float amount, Vector3 dir, bool ignoreKnockbackImmunity);
}

}
