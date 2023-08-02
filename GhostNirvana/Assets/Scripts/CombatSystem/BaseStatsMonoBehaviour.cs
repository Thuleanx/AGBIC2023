using UnityEngine;
using NaughtyAttributes;

namespace CombatSystem {

public class BaseStatsMonoBehaviour : MonoBehaviour {
    [SerializeField, Expandable] public BaseStats Stats;
}

}
