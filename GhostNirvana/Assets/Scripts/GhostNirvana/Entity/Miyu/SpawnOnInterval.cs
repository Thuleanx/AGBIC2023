using UnityEngine;
using NaughtyAttributes;
using ScriptableBehaviour;
using Optimization;
using CombatSystem;
using FMODUnity;

namespace GhostNirvana {

public class SpawnOnInterval : MonoBehaviour {
    [SerializeField] Miyu miyu;
    [SerializeField] ScriptableFloat spawnSpeed;
    [SerializeField, ShowAssetPreview, Required] GameObject prefab;
    [SerializeField] ScriptableFloat damageScaling;
    [SerializeField] ScriptableInt bulletDamage;
    [SerializeField] ScriptableFloat bulletKnockback;
    [SerializeField] EventReference onSpawnSFX;

    float spawnProgress = 0;

    void Awake() {
        miyu = GetComponentInParent<Miyu>();
    }

    void Update() {
        if (miyu.IsDead) return;
        if (!spawnSpeed || !prefab || spawnSpeed.Value == 0) return;
        spawnProgress += Time.deltaTime * spawnSpeed.Value;
        while (spawnProgress >= 1) {
            Spawn();
            spawnProgress--;
        }
    }

    void Spawn() {
        GenericHitResponder hitResponder = ObjectPoolManager.Instance.Borrow(
            gameObject.scene, prefab.transform,
            transform.position, Quaternion.identity
        ).GetComponent<GenericHitResponder>();

        FMODUnity.RuntimeManager.PlayOneShotAttached(onSpawnSFX, hitResponder.gameObject);

        int damage = Mathf.CeilToInt(damageScaling.Value * bulletDamage.Value);
        hitResponder.Initialize(damage, bulletKnockback.Value);
    }
}

}
