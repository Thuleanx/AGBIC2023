using UnityEngine;
using System.Collections.Generic;
using NaughtyAttributes;

namespace GhostNirvana {
    public class ApplianceCollector : MonoBehaviour {
        [SerializeField] Bank bank;
        [SerializeField] MovableAgentRuntimeSet appliances;
        [field:SerializeField, ReadOnly] public int numCollected { get; private set; }

        public (int,int) Collect(int applianceCount) {
            List<Appliance> applianceToCollect = new List<Appliance>();

            foreach (Appliance appliance in appliances) {
                if (applianceToCollect.Count >= applianceCount) break;
                if (appliance.IsPossessedByGhost) continue;
                applianceToCollect.Add(appliance);
            }
            int applianceCollected = applianceToCollect.Count;
            int totalMoneyEarned = 0;
            foreach (Appliance appliance in applianceToCollect) {
                totalMoneyEarned += appliance.Price;
                appliances.Remove(appliance);
                appliance.ApplianceCollectorOnly_Collect();
                bank.Deposit(appliance.Price);
            }
            numCollected += applianceCollected;
            return (applianceCollected, totalMoneyEarned);
        }
    }
}
