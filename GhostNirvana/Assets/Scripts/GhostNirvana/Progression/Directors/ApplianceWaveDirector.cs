using UnityEngine;
using NaughtyAttributes;
using CombatSystem;
using System.Collections.Generic;
using Utils;

namespace GhostNirvana {

public class ApplianceWaveDirector : Director {
    [SerializeField] WaveDirector waveDirector;
    [SerializeField] List<Appliance> appliances;
    [SerializeField] List<float> applianceSpawnRate;
    [SerializeField] BaseStats applianceBaseStats;

    [SerializeField, Range(0, 3f)] float healthScaling = 1;
    [SerializeField, Range(0, 3f)] float movementScaling = 1;
    [SerializeField, Range(0, 3f)] float accelerationScaling = 1;

    [SerializeField] List<BoxCollider> spawnAreas;

    int currentWave;
    float cumulativeWaveTime;
    float enemyNeedsToSpawn = 0;
    float lastFrameTime = 0;

    protected void OnEnable() {
        currentWave = 0;
    }

    protected void Update() {
        if (applianceSpawnRate.Count == 0) return;
        List<WaveDirector.Wave> allWaves = waveDirector.allWaves;
        while (currentWave < applianceSpawnRate.Count) {
            bool currentWaveFinished = cumulativeWaveTime + 
                allWaves[currentWave].duration <= timeElapsed.Value;
            if (!currentWaveFinished) break;

            cumulativeWaveTime += allWaves[currentWave].duration;
            currentWave++;
        }

        WaveDirector.Wave wave = currentWave < allWaves.Count ? allWaves[currentWave] : allWaves[allWaves.Count-1];

        float timeSinceLastWave = timeElapsed.Value - cumulativeWaveTime;
		float secondsInMinute = 60;
        float deltaTime = (timeElapsed.Value - lastFrameTime) * secondsInMinute;
        enemyNeedsToSpawn += deltaTime * applianceSpawnRate[currentWave] / secondsInMinute;

        if (enemyNeedsToSpawn > 0)
        while (enemyNeedsToSpawn --> 0) {
            var (prefab, baseStats) = wave.GetMob();
            GameObject spawnedEnemy = SpawnEnemy(prefab, baseStats, GetRandomPosition());
            SpawnAppliance(spawnedEnemy);
        }

        lastFrameTime = timeElapsed.Value;
    }

    Vector3 GetRandomPosition() {
        float totalArea = 0;
        foreach (BoxCollider spawnArea in spawnAreas)
            totalArea += spawnArea.size.x * spawnArea.size.z * spawnArea.transform.lossyScale.x * spawnArea.transform.lossyScale.z;
        float randomPoint = Mathx.RandomRange(0, totalArea);
        foreach (BoxCollider spawnArea in spawnAreas) {
            float area = spawnArea.size.x * spawnArea.size.z * spawnArea.transform.lossyScale.x * spawnArea.transform.lossyScale.z;
            totalArea -= area;
            if (totalArea <= 0) {
                Vector3 localPos = new Vector3(
                    (0.5f - Mathx.RandomRange(0.0f,1.0f)) * spawnArea.size.x,
                    0,
                    (0.5f - Mathx.RandomRange(0.0f,1.0f)) * spawnArea.size.z);

                Vector3 worldPos = spawnArea.transform.TransformPoint(localPos);
                worldPos.y = 0;
                return worldPos;
            }
        }
        return Arena.Instance.GetRandomLocationInExtents();
    }

    void SpawnAppliance(GameObject enemy) {
        BaseStats enemyStats = enemy.GetComponent<BaseStatsMonoBehaviour>().Stats;
        applianceBaseStats.MaxHealth = Mathf.CeilToInt(enemyStats.MaxHealth * healthScaling);
        applianceBaseStats.MovementSpeed = enemyStats.MovementSpeed * movementScaling;
        applianceBaseStats.Acceleration = enemyStats.Acceleration * accelerationScaling;

        Appliance randomAppliance = appliances[Mathx.RandomRange(0, appliances.Count)];

        SpawnEnemy(randomAppliance.gameObject, applianceBaseStats, enemy.transform.position);
    }
}

}
