using Base;
using UnityEngine;
using CombatSystem;
using ScriptableBehaviour;
using Optimization;
using Utils;
using FMODUnity;

namespace GhostNirvana {

public class SpawnOnHit : MonoBehaviour {
    [SerializeField] GameObject prefab;
    [SerializeField] ScriptableInt numberOfHitsToSpawn;
    [SerializeField] ScriptableFloat damageScaling;
    [SerializeField] ScriptableInt bulletDamage;
    [SerializeField] ScriptableFloat bulletKnockback;
    [SerializeField] EventReference onSpawnSFX;
    Miyu miyu;

    int hitCount;

    void Awake() {
        miyu = GetComponentInParent<Miyu>();
    }

    protected void OnEnable() {
        miyu.OnHitEvent?.AddListener(OnHit);
    }
    
    protected void OnDisable() {
        miyu.OnHitEvent?.RemoveListener(OnHit);
    }

    void OnHit(Hit hit) {
        if (numberOfHitsToSpawn.Value == 0) return;

        hitCount++;
        if (hitCount < numberOfHitsToSpawn.Value) return;
        hitCount = 0;

        /* float spawnChance = chanceToSpawnOnHit ? chanceToSpawnOnHit.Value : 0; */
        /* if (spawnChance <= 0) return; */
        /* spawnChance = Mathf.Min(spawnChance, 1); */

        /* bool shouldSpawn = Mathx.RandomRange(0.0f, 1.0f) < spawnChance; */
        /* if (!shouldSpawn) return; */

        Vector3 spawnPosition = (hit.Hurtbox as MonoBehaviour)?.transform?.position ?? hit.Position;
        spawnPosition.y = 0;

        GenericHitResponder hitResponder = ObjectPoolManager.Instance.Borrow(
            gameObject.scene, prefab.transform,
            spawnPosition, Quaternion.identity
        ).GetComponent<GenericHitResponder>();

        FMODUnity.RuntimeManager.PlayOneShotAttached(onSpawnSFX, hitResponder.gameObject);

        int damage = Mathf.CeilToInt(damageScaling.Value * bulletDamage.Value);
        hitResponder.Initialize(damage, bulletKnockback.Value);
    }
}

}
