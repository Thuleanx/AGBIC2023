using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ScriptableBehaviour;
using NaughtyAttributes;

namespace GhostNirvana {
    public class TemporaryBuffHolder : MonoBehaviour {
        [SerializeField] List<(LinearInt, TemporaryBuff)> allBuffs;

        public void ApplyAllBuffs() {
            foreach (var (numberOfTimesToApply, buff) in allBuffs) {
                int count = numberOfTimesToApply.Value;
                while (count --> 0) buff.ApplyOnHost(this);
            }
        }
    }
}
