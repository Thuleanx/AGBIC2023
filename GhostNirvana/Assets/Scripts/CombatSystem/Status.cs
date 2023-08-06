using UnityEngine;
using UnityEngine.Events;
using Base;
using NaughtyAttributes;

namespace CombatSystem {

[System.Serializable]
[RequireComponent(typeof(BaseStatsMonoBehaviour))]
public class Status : MonoBehaviour {
    [field:SerializeField, Required]
    public BaseStatsMonoBehaviour BaseStatsHolder {
        get;
        protected set;
    }

    public BaseStats BaseStats => BaseStatsHolder.Stats ?? null;

    [field:SerializeField, ReadOnly]
    public int Health {
        get;
        protected set;
    }

    [HideInInspector] public Entity Owner;

    public bool IsDead => Health <= 0;
    public UnityEvent<Status> OnDeath;

    protected void Awake() {
        BaseStatsHolder = GetComponent<BaseStatsMonoBehaviour>();
    }

    protected void OnEnable() {
        HealToFull();
    }

    public void TakeDamage(int amount) {
		if (IsDead) return;
        Health -= amount;
        if (Health <= 0) OnDeath?.Invoke(this);
    }

    public void HealToFull() => Health = BaseStatsHolder.Stats.MaxHealth;
    public void SetHealth(int health) => Health = Mathf.Clamp(health, 0, BaseStats.MaxHealth);
}

}
