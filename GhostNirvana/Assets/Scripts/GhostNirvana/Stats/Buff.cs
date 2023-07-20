using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace GhostNirvana {
    [CreateAssetMenu(fileName = "Buff",
                 menuName = "~/Stats/Buff", order = 1)]
    public class Buff : ScriptableObject {
        [System.Serializable]
        struct LinearBuff<T> where T : ScriptableFloat {
            [field:SerializeField, Expandable]
            public T Stat { get; private set; }
            [field:SerializeField]
            public float AdditiveAmount {get; private set; }
            [field:SerializeField]
            public float MultiplicativeAmount {get; private set; }
        };
        [SerializeField, ReorderableList] List<LinearBuff<LinearFloat>> linearFloatBuffs;
        [SerializeField, ReorderableList] List<LinearBuff<LinearLimiterFloat>> linearLimiterFloatBuffs;

        public void Apply() {
            foreach (LinearBuff<LinearFloat> buff in linearFloatBuffs) {
                buff.Stat.AdditiveScale += buff.AdditiveAmount;
                buff.Stat.MultiplicativeScale *= buff.MultiplicativeAmount;
            }
            foreach (LinearBuff<LinearLimiterFloat> buff in linearLimiterFloatBuffs) {
                buff.Stat.AdditiveScale += buff.AdditiveAmount;
                buff.Stat.MultiplicativeScale *= buff.MultiplicativeAmount;
            }
        }
    }
}
