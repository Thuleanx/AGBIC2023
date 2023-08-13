using UnityEngine;
using UnityEngine.Events;
using ScriptableBehaviour;

namespace GhostNirvana {
   [CreateAssetMenu(fileName = "Bank",
                 menuName = "~/Progression/Bank", order = 1)]
    public class Bank : Scriptable<int> {
        public int Balance { get => Value; private set => Value = value; }

        public UnityEvent<int> OnWithraw;
        public UnityEvent<int> OnDeposit;

        public void Deposit(int currency) {
            Balance += currency;
            OnDeposit?.Invoke(currency);
        }
        public void Withraw(int currency) {
            Balance -= currency;
            OnWithraw?.Invoke(currency);
        }

        public virtual void OnAfterDeserialize() {
            OnDeposit?.RemoveAllListeners();
            OnWithraw?.RemoveAllListeners();
        }

        public virtual void OnBeforeSerialize() {
            OnDeposit?.RemoveAllListeners();
            OnWithraw?.RemoveAllListeners();
        }
    }
}
