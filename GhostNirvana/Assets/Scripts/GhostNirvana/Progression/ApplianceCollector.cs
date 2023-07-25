using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GhostNirvana {
    public class ApplianceCollector : MonoBehaviour {
        [SerializeField] Bank bank;
        [SerializeField] MovableAgentRuntimeSet appliances;

        public void Collect(int applianceCount) {
            List<Appliance> applianceToCollect = new List<Appliance>();
            foreach (Appliance appliance in appliances) {
                if (applianceToCollect.Count >= applianceCount) break;
                if (appliance.IsPossessed) continue;
                applianceToCollect.Add(appliance);
            }
            foreach (Appliance appliance in applianceToCollect) {
                appliances.Remove(appliance);
                appliance.ApplianceCollectorOnly_Collect();
                bank.Deposit(appliance.Price);
            }
        }
    }
}
