using UnityEngine;
using Base;
using System.Collections.Generic;
using ScriptableBehaviour;

namespace GhostNirvana {
    public class TemporaryBuffHolder : MonoBehaviour {
        [System.Serializable]
        struct TemporaryBuffApplication {
            public LinearInt numberOfTimesToApply;
            public TemporaryBuff buff;

            public void Deconstruct(out LinearInt numberOfTimesToApply, out TemporaryBuff buff) {
                numberOfTimesToApply = this.numberOfTimesToApply;
                buff = this.buff;
            }
        }

        [SerializeField] List<TemporaryBuffApplication> allBuffs;

        public void ApplyAllBuffs() {
            foreach (var (numberOfTimesToApply, buff) in allBuffs) {
                int count = numberOfTimesToApply?.Value ?? 1;
                while (count --> 0) buff.ApplyOnHost(Upgrade.UpgradeSystem.Instance);
            }
        }

        void OnDisable() {
            foreach (var (numberOfTimesToApply, buff) in allBuffs) {
                int count = numberOfTimesToApply?.Value ?? 1;
                if (buff.IsActive) buff.ForceRemoveBuff();
            }
        }
    }
}
