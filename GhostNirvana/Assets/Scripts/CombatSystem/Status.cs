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
    public float Health {
        get;
        protected set;
    }

    [HideInInspector] public Entity Owner;

    public bool IsDead => Health <= 0;
    public UnityEvent<Status> OnDeath;

    protected void Awake() {
        BaseStatsHolder = GetComponent<BaseStatsMonoBehaviour>();
    }

    public void TakeDamage(float amount) {
		if (IsDead) return;
        Health -= amount;
        if (Health <= 0) OnDeath?.Invoke(this);
    }

    public void HealToFull() => Health = BaseStatsHolder.Stats.MaxHealth;
}

}
