
using UnityEngine;
using CombatSystem;
using System.Collections.Generic;

namespace GhostNirvana.CombatSystem {
    public class ForgetfulHitbox : Hitbox {
        [SerializeField] float forgetTime = 0.5f;
        Dictionary<IHurtbox, float> hurtboxes = new Dictionary<IHurtbox, float>();

        protected virtual void OnDisable() => hurtboxes.Clear();

        protected override bool ValidateHit(Hit hit) {
            bool notHitWithinTimeFrame = !hurtboxes.ContainsKey(hit.Hurtbox)
                || hurtboxes[hit.Hurtbox] + forgetTime < Time.time;

            hurtboxes[hit.Hurtbox] = Time.time;

            return notHitWithinTimeFrame;
        }
    }
}
