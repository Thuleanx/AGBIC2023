using UnityEngine;

namespace GhostNirvana {
   [CreateAssetMenu(fileName = "Bank",
                 menuName = "~/Progression/Bank", order = 1)]
    public class Bank : ScriptableObject {
        [field:System.NonSerialized]
        public int Balance { get; private set; }

        public void Deposit(int currency) => Balance += currency;

        public void Withraw(int currency) => Balance -= currency;
    }
}
