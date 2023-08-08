using UnityEngine;
using System.Collections.Generic;

namespace GhostNirvana {
    public class ApplianceCollector : MonoBehaviour {
        [SerializeField] Bank bank;
        [SerializeField] MovableAgentRuntimeSet appliances;

        public (int,int) Collect(int applianceCount) {
            List<Appliance> applianceToCollect = new List<Appliance>();

            foreach (Appliance appliance in appliances) {
                if (applianceToCollect.Count >= applianceCount) break;
                if (appliance.IsPossessed) continue;
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
            return (applianceCollected, totalMoneyEarned);
        }
    }
}
