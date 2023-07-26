using UnityEngine;
using ScriptableBehaviour;

namespace GhostNirvana {
   [CreateAssetMenu(fileName = "Bank",
                 menuName = "~/Progression/Bank", order = 1)]
    public class Bank : Scriptable<int> {
        public int Balance { get => Value; private set => Value = value; }

        public void Deposit(int currency) => Balance += currency;
        public void Withraw(int currency) => Balance -= currency;
    }
}
