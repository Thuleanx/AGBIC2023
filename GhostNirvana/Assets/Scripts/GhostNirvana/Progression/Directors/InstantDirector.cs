using Base;
using UnityEngine;
using NaughtyAttributes;
using CombatSystem;
using System.Collections.Generic;
using Utils;

namespace GhostNirvana {

public class InstantDirector : Director {
    [System.Serializable]
    public struct SpawnOption {
        [ShowAssetPreview] public GameObject enemy;
        [Expandable] public BaseStats baseStats;
    }
    [SerializeField] List<SpawnOption> spawnOptions;
    [SerializeField] int enemiesToSpawn;

    public void Spawn() {
        int enemiesLeftToSpawn = enemiesToSpawn;
        while (enemiesLeftToSpawn-->0) {
            SpawnOption spawnOption = spawnOptions[Mathx.RandomRange(0, spawnOptions.Count)];
            SpawnEnemy(spawnOption.enemy, spawnOption.baseStats);
        }
        Destroy(gameObject);
    }
}

}
