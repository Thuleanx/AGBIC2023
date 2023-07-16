using UnityEngine;
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

    public void TakeDamage(float amount) {
        Health -= amount;
        if (Health < 0) Health = 0;
    }

    void Awake() {
        Health = MaxHealth;
    }
}

}
