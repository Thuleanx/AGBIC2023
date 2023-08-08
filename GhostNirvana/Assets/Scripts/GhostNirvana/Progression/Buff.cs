using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using ScriptableBehaviour;

namespace GhostNirvana {
    [CreateAssetMenu(fileName = "Buff",
                 menuName = "~/Stats/Buff", order = 1)]
    public class Buff : ScriptableObject {
        [System.Serializable]
        struct LinearBuff<T, ST> where T : ScriptableObject, ILinearlyScalable<ST> {
            [field:SerializeField, Expandable]
            public T Stat { get; private set; }
            [field:SerializeField]
            public float AdditiveAmount {get; private set; }
            [field:SerializeField]
            public float MultiplicativeAmount {get; private set; }
        };

        [System.Serializable]
        struct Regain<T, ST> where T : ScriptableObject, ILimited<ST> {
            [field:SerializeField, Expandable]
            public T Stat { get; private set; }
            [field:SerializeField]
            public bool All {get; private set; }
            [field:SerializeField, HideIf("All")]
            public ST Amount {get; private set; }
        }
        [SerializeField, ReorderableList] List<LinearBuff<LinearFloat, float>> floatBuffs;
        [SerializeField, ReorderableList] List<LinearBuff<LinearInt, int>> intBuffs;
        [SerializeField, ReorderableList] List<Regain<LinearLimiterFloat, float>> replenishFloat;
        [SerializeField, ReorderableList] List<Regain<LinearLimiterInt, int>> replenishInt;

        [field:SerializeField, ResizableTextArea] public string description {get; private set; }
        [field:SerializeField] public float Cost {get; private set; }
        [field:SerializeField] public float Weight {get; private set; }
        [field:SerializeField] public int purchaseLimit;

        [ReorderableList] public List<Buff> Prerequisites = new List<Buff>();

        public void Apply() {
            foreach (LinearBuff<LinearFloat, float> buff in floatBuffs) {
                ILinearlyScalable<float> stat = buff.Stat;
                stat.AdditiveScale += buff.AdditiveAmount;
                stat.MultiplicativeScale *= buff.MultiplicativeAmount;
                stat.Recompute();
            }
            foreach (LinearBuff<LinearInt, int> buff in intBuffs) {
                ILinearlyScalable<int> stat = buff.Stat;
                stat.AdditiveScale += buff.AdditiveAmount;
                stat.MultiplicativeScale *= buff.MultiplicativeAmount;
                stat.Recompute();
            }
            foreach (Regain<LinearLimiterFloat, float> replenish in replenishFloat) {
                replenish.Stat.Value += replenish.Amount;
                replenish.Stat.CheckAndCorrectLimit();
            }
            foreach (Regain<LinearLimiterInt, int> replenish in replenishInt) {
                replenish.Stat.Value += replenish.Amount;
                replenish.Stat.CheckAndCorrectLimit();
            }
        }

        public float ComputeWeight(int value, bool hasPrerequisites) {
            return 0;
        }
    }
}
