using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using ScriptableBehaviour;

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

        [System.Serializable]
        struct Regain<T> where T : LinearLimiterFloat {
            [field:SerializeField, Expandable]
            public T Stat { get; private set; }
            [field:SerializeField]
            public bool All {get; private set; }
            [field:SerializeField, HideIf("All")]
            public float Amount {get; private set; }
        }
        [SerializeField, ReorderableList] List<LinearBuff<LinearFloat>> linearFloatBuffs;
        [SerializeField, ReorderableList] List<LinearBuff<LinearLimiterFloat>> linearLimiterFloatBuffs;
        [SerializeField, ReorderableList] List<Regain<LinearLimiterFloat>> replenishBuffs;

        [field:SerializeField, ResizableTextArea] public string description {get; private set; }
        [field:SerializeField] public float cost {get; private set; }
        [field:SerializeField] public int purchaseLimit;

        [ReorderableList] public List<Buff> Prerequisites = new List<Buff>();

        public void Apply() {
            foreach (LinearBuff<LinearFloat> buff in linearFloatBuffs) {
                buff.Stat.AdditiveScale += buff.AdditiveAmount;
                buff.Stat.MultiplicativeScale *= buff.MultiplicativeAmount;
                buff.Stat.Recompute();
            }
            foreach (LinearBuff<LinearLimiterFloat> buff in linearLimiterFloatBuffs) {
                buff.Stat.AdditiveScale += buff.AdditiveAmount;
                buff.Stat.MultiplicativeScale *= buff.MultiplicativeAmount;
                buff.Stat.Recompute();
            }
            foreach (Regain<LinearLimiterFloat> replenish in replenishBuffs) {
                replenish.Stat.Value += replenish.Amount;
                replenish.Stat.CheckAndCorrectLimit();
            }
        }
    }
}
