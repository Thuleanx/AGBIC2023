using UnityEngine;

namespace CombatSystem {

public abstract class DamageType {}

public struct Hit {
    public readonly Vector3 Position;
    public readonly Vector3 Normal;
    public readonly float Time;
    public readonly IHitbox Hitbox;
    public readonly IHurtbox Hurtbox;

    public IHitResponder Causer => Hitbox.HitResponder;
    public IHurtResponder Receiver => Hurtbox.HurtResponder;

    public Hit(Vector3 position, Vector3 normal, float time, IHitbox hitbox, IHurtbox hurtbox) {
        Position = position;
        Normal = normal;
        Time = time;
        Hitbox = hitbox;
        Hurtbox = hurtbox;
    }
}

}
