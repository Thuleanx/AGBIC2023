using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace GhostNirvana {
    [CreateAssetMenu(fileName = "Cheat",
                 menuName = "~/Progression/Cheat", order = 1)]
    public class ProgressionCheat : ScriptableObject {
        public List<Buff> BuffsTakenInSequence = new List<Buff>();
        [SerializeField] Buff onLevelUpBuff;

        [Button]
        void ApplyCheat() {
            foreach (Buff buff in BuffsTakenInSequence) {
                buff.Apply();
                onLevelUpBuff?.Apply();
            }
        }
    }
}
