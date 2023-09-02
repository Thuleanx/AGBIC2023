using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using ScriptableBehaviour;

namespace GhostNirvana {
    [CreateAssetMenu(fileName = "StatsList",
                 menuName = "~/Stats/StatsList", order = 1)]
    public class StatsList : ScriptableObject {
        [ReorderableList] public List<LinearInt> AllInts;
        [ReorderableList] public List<LinearFloat> AllFloats;
    }
}
