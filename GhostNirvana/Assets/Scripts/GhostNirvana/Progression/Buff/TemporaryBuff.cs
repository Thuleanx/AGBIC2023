using System;
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
        [NonSerialized] bool buffCurrentlyActive;
        [NonSerialized] float timeExpire;

        [HideInInspector] public UnityEvent OnApply;
        [HideInInspector] public UnityEvent OnExpire;

        public bool IsActive => buffCurrentlyActive;

        public void ApplyOnHost(MonoBehaviour host) {
            if (!buffCurrentlyActive) OnApply?.Invoke();
            ExtendDuration();
            if (stackable || !buffCurrentlyActive)
                host.StartCoroutine(_BuffDuration());
        }

        void ExtendDuration() {
            float durationUse = useScriptableDuration ?
                (scriptableDuration?.Value ?? duration) : duration;
            timeExpire = Time.time + durationUse;
        }

        IEnumerator _BuffDuration() {
            Apply();
            buffCurrentlyActive = true;
            while (Time.time < timeExpire)
                yield return null;
            buffCurrentlyActive = false;
            Revert();
            OnExpire?.Invoke();
        }

        public virtual void OnAfterDeserialize() {
            buffCurrentlyActive = false;
            timeExpire = 0;
            OnApply?.RemoveAllListeners();
            OnExpire?.RemoveAllListeners();
        }

        public virtual void OnBeforeSerialize() {
        }

        public void ForceRemoveBuff() {
            if (!buffCurrentlyActive) return;
            buffCurrentlyActive = false;
            Revert();
            OnExpire?.Invoke();
        }
    }
}
