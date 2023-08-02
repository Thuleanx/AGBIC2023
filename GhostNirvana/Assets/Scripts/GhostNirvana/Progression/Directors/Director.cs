using UnityEngine;
using CombatSystem;
using Optimization;
using ScriptableBehaviour;

namespace GhostNirvana {

public abstract class Director : MonoBehaviour {
    [SerializeField] protected ScriptableFloat timeElapsed;

    protected void SpawnEnemy(GameObject enemy, BaseStats baseStats) {
        PoolableEntity poolableEnemy = enemy.GetComponent<PoolableEntity>();
        Vector3 spawnPoint = Arena.Instance?.GetRandomLocationInExtents() ?? Vector3.zero;

        PoolableEntity spawnedEnemy = ObjectPoolManager.Instance.Borrow(
            gameObject.scene, 
            poolableEnemy,
            spawnPoint
        );

        BaseStatsMonoBehaviour baseStatsHolder = spawnedEnemy.GetComponent<BaseStatsMonoBehaviour>();
        baseStatsHolder.Stats = baseStats;
    }
}

}
