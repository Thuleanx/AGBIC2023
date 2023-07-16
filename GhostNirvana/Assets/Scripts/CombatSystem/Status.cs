using UnityEngine;
using UnityEngine.Events;
using Base;
using NaughtyAttributes;

namespace CombatSystem {

public class Status : MonoBehaviour {
    [field:SerializeField]
    public float MaxHealth {
        get;
        protected set;
    }

    [field:SerializeField, ReadOnly]
    public float Health {
        get;
        protected set;
    }

    public bool IsDead => Health <= 0;
    public UnityEvent OnDeath;

    public void TakeDamage(float amount) {
        bool isKillingHit = !IsDead && amount >= Health;

        Health -= amount;
        if (Health <= 0) Health = 0;

        if (isKillingHit) OnDeath?.Invoke();
    }

    void Awake() {
        Health = MaxHealth;
    }
}

}
