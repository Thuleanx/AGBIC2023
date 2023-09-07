using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using ScriptableBehaviour;
using NaughtyAttributes;

namespace GhostNirvana {
    [CreateAssetMenu(fileName = "TemporaryBuff",
                 menuName = "~/Stats/TemporaryBuff", order = 1)]
    public class TemporaryBuff : Buff, ISerializationCallbackReceiver {
        [SerializeField] bool useScriptableDuration;
        [SerializeField, HideIf("useScriptableDuration")] float duration;
        [SerializeField, ShowIf("useScriptableDuration")] LinearFloat scriptableDuration;

        [SerializeField] bool stackable;
        bool buffCurrentlyActive;
        Coroutine currentCoroutine;

        [HideInInspector] public UnityEvent OnApply;
        [HideInInspector] public UnityEvent OnExpire;

        public bool IsActive => buffCurrentlyActive;

        public void ApplyOnHost(MonoBehaviour host) {
            if (!buffCurrentlyActive) OnApply?.Invoke();
            if (currentCoroutine != null && !stackable && buffCurrentlyActive) {
                Revert();
                host.StopCoroutine(currentCoroutine);
                buffCurrentlyActive = false;
            }
            currentCoroutine = host.StartCoroutine(_BuffDuration());
        }

        IEnumerator _BuffDuration() {
            Apply();
            buffCurrentlyActive = true;
            float durationUse = useScriptableDuration ?
                (scriptableDuration?.Value ?? duration) : duration;
            yield return new WaitForSeconds(durationUse);
            Revert();
            OnExpire?.Invoke();
            buffCurrentlyActive = false;
        }

        public virtual void OnAfterDeserialize() {
            currentCoroutine = null;
            buffCurrentlyActive = false;
            OnApply?.RemoveAllListeners();
            OnExpire?.RemoveAllListeners();
        }

        public virtual void OnBeforeSerialize() {
        }
    }
}
