using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ScriptableBehaviour;
using NaughtyAttributes;

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
                int count = numberOfTimesToApply.Value;
                while (count --> 0) buff.ApplyOnHost(this);
            }
        }
    }
}
