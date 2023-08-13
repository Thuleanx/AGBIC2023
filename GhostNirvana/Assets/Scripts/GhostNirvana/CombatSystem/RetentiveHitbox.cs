using UnityEngine;
using CombatSystem;
using System.Collections.Generic;

namespace GhostNirvana.CombatSystem {
    public class RetentiveHitbox : Hitbox {
        HashSet<IHurtbox> hurtboxes = new HashSet<IHurtbox>();

        protected virtual void OnDisable() => hurtboxes.Clear();

        protected override bool ValidateHit(Hit hit) {
            bool alreadyHit = hurtboxes.Contains(hit.Hurtbox);
            hurtboxes.Add(hit.Hurtbox);
            return !alreadyHit;
        }
    }
}
