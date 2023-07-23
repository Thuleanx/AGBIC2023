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

    public static void ConnectChildrenHitboxes<T>(T responder) where T : MonoBehaviour, IHitResponder {
        foreach (IHitbox hurtbox in responder.GetComponentsInChildren<IHitbox>())
            hurtbox.HitResponder = responder;
    }

    public static void DisconnectChildrenHitboxes<T>(T responder) where T : MonoBehaviour, IHitResponder {
        foreach (IHitbox hurtbox in responder.GetComponentsInChildren<IHitbox>())
            hurtbox.HitResponder = responder;
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
    public void RespondToHit(Hit hit);

    public static void ConnectChildrenHurtboxes<T>(T responder) where T : MonoBehaviour, IHurtResponder {
        foreach (IHurtbox hurtbox in responder.GetComponentsInChildren<IHurtbox>())
            hurtbox.HurtResponder = responder;
    }

    public static void DisconnectChildrenHurtboxes<T>(T responder) where T : MonoBehaviour, IHurtResponder {
        foreach (IHurtbox hurtbox in responder.GetComponentsInChildren<IHurtbox>())
            hurtbox.HurtResponder = responder;
    }
}

public interface IHurtable {
    public void TakeDamage(float damageAmount, DamageType damageType, Hit hit)
        => OnTakeDamage(damageAmount, damageType, hit);

    protected void OnTakeDamage(float damageAmount,
                                DamageType damageType,
                                Hit hit);
}

}
