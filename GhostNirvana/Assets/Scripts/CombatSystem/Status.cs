using UnityEngine;
using UnityEngine.Events;
using Base;
using NaughtyAttributes;

namespace CombatSystem {

[System.Serializable]
public class Status {
    [field:SerializeField, Expandable, Required]
    public BaseStats BaseStats {
        get;
        protected set; 
    }

    [field:SerializeField, ReadOnly]
    public float Health {
        get;
        protected set;
    }

    [HideInInspector] public Entity Owner;

    public bool IsDead => Health <= 0;
    public UnityEvent OnDeath;

    public void TakeDamage(float amount) {
        bool isKillingHit = !IsDead && amount >= Health;

        Health -= amount;
        if (Health <= 0) Health = 0;

        if (isKillingHit) OnDeath?.Invoke();
    }
}

}
