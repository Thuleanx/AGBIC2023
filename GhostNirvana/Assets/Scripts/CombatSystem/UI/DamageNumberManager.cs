using UnityEngine;
using System.Collections.Generic;

namespace CombatSystem.UI {

[RequireComponent(typeof(DamageNumber))]
class DamageNumberBelongsToPool : MonoBehaviour {
    DamageNumber damageNumber;
    DamageNumberManager manager;

    void Awake() {
        damageNumber = GetComponent<DamageNumber>();
    }

    public void SetManager(DamageNumberManager manager) => this.manager = manager;
    void OnDisable() => manager?.Collect(damageNumber);
}

public class DamageNumberManager : MonoBehaviour {
    [SerializeField] DamageNumber damageNumberPrefab;
    [SerializeField] int poolExpansionRate = 20;
    [SerializeField] HurtableRuntimeSet hurtables;

    Queue<DamageNumber> damageNumberPool = new Queue<DamageNumber>();

    void OnEnable() {
        hurtables.OnAdd += OnHurtableAdd;
        hurtables.OnRemove += OnHurtableRemove;
        foreach (IHurtable hurtable in hurtables)
            OnHurtableAdd(hurtable);
    }

    void OnDisable() {
        hurtables.OnAdd -= OnHurtableAdd;
        hurtables.OnRemove -= OnHurtableRemove;
        foreach (IHurtable hurtable in hurtables)
            OnHurtableRemove(hurtable);
    }

    void OnHurtableAdd(IHurtable ihurtable) {
        ihurtable.OnDamage.AddListener(OnDamageTaken);
    }
    void OnHurtableRemove(IHurtable ihurtable) => ihurtable.OnDamage.RemoveListener(OnDamageTaken);

    void Expand(int amount) {
        bool prefabIsActive = damageNumberPrefab.gameObject.activeSelf;
        damageNumberPrefab.gameObject.SetActive(false);

        while (amount --> 0) {
            DamageNumber damageNumber = Instantiate(damageNumberPrefab, parent: transform);
            DamageNumberBelongsToPool belongsToPool = damageNumber.gameObject.AddComponent<DamageNumberBelongsToPool>();
            belongsToPool.SetManager(this);
            damageNumberPool.Enqueue(damageNumber);
        }
        damageNumberPrefab.gameObject.SetActive(prefabIsActive);
    }

    void OnDamageTaken(IHurtable hurtable, float damage, DamageType damageType) {
        Debug.Log("HI");
        if (!(hurtable is MonoBehaviour)) return;
        if (damageNumberPool.Count == 0) Expand(poolExpansionRate);

        DamageNumber damageNumber = damageNumberPool.Dequeue();
        damageNumber.gameObject.SetActive(true);
        damageNumber.Initialize(damage, (hurtable as MonoBehaviour).transform.position);
    }

    public void Collect(DamageNumber number) => damageNumberPool.Enqueue(number);
}

}
