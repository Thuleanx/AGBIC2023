using UnityEngine;
using ScriptableBehaviour;

namespace CombatSystem {

[CreateAssetMenu(fileName = "StatusSet",
                 menuName = "~/Sets/Status", order = 1)]
public class StatusRuntimeSet : RuntimeSet<Status> {}

}
