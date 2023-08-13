using UnityEngine;
using NaughtyAttributes;
using CombatSystem;
using System.Collections.Generic;
using Utils;

namespace GhostNirvana {

public class WaveDirector : Director {
    [System.Serializable]
    public class Wave {
        [System.Serializable]
        public class SpawnableMob {
            [ShowAssetPreview] public GameObject enemy;
            [Expandable]
            public BaseStats baseStats;
            public float weight;
        }

        [SerializeField] List<SpawnableMob> spawnableMobs;
        [SerializeField] float spawnFrequencyFixed;
        [SerializeField] public float duration = 1;

        public (GameObject, BaseStats) GetMob() {
            if (spawnableMobs.Count == 0)
                return (null, null);

            float totalWeight = 0;
            foreach (SpawnableMob spawnOption in spawnableMobs)
                totalWeight += spawnOption.weight;
            float chosen = Mathx.RandomRange(0, totalWeight);

            SpawnableMob chosenMob = spawnableMobs[spawnableMobs.Count - 1];
            foreach (SpawnableMob candidateMob in spawnableMobs) {
                chosen -= candidateMob.weight;
                if (chosen <= 0) {
                    chosenMob = candidateMob;
                    break;
                }
            }

            return (chosenMob.enemy, chosenMob.baseStats);
        }

        public float GetSpawnFrequency(float time)
            => spawnFrequencyFixed;
    }

    [SerializeField] public List<Wave> allWaves;
    int currentWave;
    float cumulativeWaveTime;
    float enemyNeedsToSpawn = 0;
    float lastFrameTime = 0;

    protected void OnEnable() {
        currentWave = 0;
    }

    protected void Update() {
        if (allWaves.Count == 0) return;
        while (currentWave < allWaves.Count) {
            bool currentWaveFinished = cumulativeWaveTime + 
                allWaves[currentWave].duration <= timeElapsed.Value;
            if (!currentWaveFinished) break;

            cumulativeWaveTime += allWaves[currentWave].duration;
            currentWave++;
        }

        Wave wave = currentWave < allWaves.Count ? allWaves[currentWave] : allWaves[allWaves.Count-1];

        float timeSinceLastWave = timeElapsed.Value - cumulativeWaveTime;
		float secondsInMinute = 60;
        float deltaTime = (timeElapsed.Value - lastFrameTime) * secondsInMinute;
        enemyNeedsToSpawn += deltaTime * wave.GetSpawnFrequency(timeSinceLastWave) / secondsInMinute;

        if (enemyNeedsToSpawn >= 1)
        do {
            var (prefab, baseStats) = wave.GetMob();
            SpawnEnemy(prefab, baseStats);
        } while (--enemyNeedsToSpawn >= 1);

        lastFrameTime = timeElapsed.Value;
    }
}

}
