using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

using CombatSystem;
using Optimization;
using Utils;

namespace GhostNirvana {

public class Director : MonoBehaviour {
    [System.Serializable]
    struct SpawnIntervalMapping {
        [field:SerializeField]
        public string name {get; private set; }
        [field:SerializeField, ShowAssetPreview] 
        public GameObject enemy {get; private set; }
        [field:SerializeField, Expandable]
        public BaseStats baseStats {get; private set; }
        [field:SerializeField]
        public AnimationCurve spawnWeightOverTime {get; private set; }
    }

    [SerializeField, ReorderableList]
    List<SpawnIntervalMapping> spawnIntervalsPerEnemy = new List<SpawnIntervalMapping>();

    [SerializeField] AnimationCurve spawnFrequencyOverTime;
    float enemiesToSpawn = 0;
    float timeElapsedMinutes = 0;

    void Update() {
        enemiesToSpawn += spawnFrequencyOverTime.Evaluate(timeElapsedMinutes) * Time.deltaTime;
        timeElapsedMinutes += Time.deltaTime / 60;

        if (enemiesToSpawn < 1) return;
        while (enemiesToSpawn --> 1) {
            GameObject enemy = ChooseEnemy();
            if (enemy == null) break;
            SpawnEnemy(enemy);
        }
    }

    GameObject ChooseEnemy() {
        float weightTotal = 0;
        foreach (SpawnIntervalMapping spawnInfo in spawnIntervalsPerEnemy)
            weightTotal += spawnInfo.spawnWeightOverTime.Evaluate(timeElapsedMinutes);

        float weightChosen = Mathx.RandomRange(0, weightTotal);
        foreach (SpawnIntervalMapping spawnInfo in spawnIntervalsPerEnemy) {
            weightChosen -= spawnInfo.spawnWeightOverTime.Evaluate(timeElapsedMinutes);
            if (weightChosen <= 0) {
                return spawnInfo.enemy;
            }
        }

        return null;
    }

    void SpawnEnemy(GameObject enemy) {
        PoolableEntity poolableEnemy = enemy.GetComponent<PoolableEntity>();
        Vector3 spawnPoint = Arena.Instance?.GetRandomLocationInExtents() ?? Vector3.zero;

        PoolableEntity spawnedEnemy = ObjectPoolManager.Instance.Borrow(
            gameObject.scene, 
            poolableEnemy,
            spawnPoint
        );
    }
}

}
