using UnityEngine;
using System.Collections;
using ScriptableBehaviour;
using NaughtyAttributes;

namespace GhostNirvana {
    [CreateAssetMenu(fileName = "TemporaryBuff",
                 menuName = "~/Stats/TemporaryBuff", order = 1)]
    public class TemporaryBuff : Buff {
        [SerializeField] bool useScriptableDuration;
        [SerializeField, HideIf("useScriptableDuration")] float duration;
        [SerializeField, ShowIf("useScriptableDuration")] LinearFloat scriptableDuration;

        public void ApplyOnHost(MonoBehaviour host) => host.StartCoroutine(_BuffDuration());

        IEnumerator _BuffDuration() {
            Apply();
            float durationUse = useScriptableDuration ?
                (scriptableDuration?.Value ?? duration) : duration;
            yield return new WaitForSeconds(durationUse);
            Revert();
        }
    }
}
