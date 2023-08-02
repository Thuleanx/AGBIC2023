using UnityEngine;
using NaughtyAttributes;

using CombatSystem;

namespace GhostNirvana {

[CreateAssetMenu(fileName = "Data",
                 menuName = "~/Director/SpawnPattern", order = 1)]
public class SpawnPattern : ScriptableObject {
    [field:SerializeField, ShowAssetPreview] public GameObject enemy {get; private set; }
    [SerializeField, Expandable] BaseStats baseStats;
    [SerializeField] AnimationCurve spawnWeightOverTime;

    public float GetWeight(float minute) 
        => spawnWeightOverTime.Evaluate(minute);
}

}
