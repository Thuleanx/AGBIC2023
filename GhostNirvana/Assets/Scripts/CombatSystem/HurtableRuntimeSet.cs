using UnityEngine;
using ScriptableBehaviour;

namespace CombatSystem {
[CreateAssetMenu(fileName = "Hurtable",
                 menuName = "~/Sets/Hurtable", order = 1)]
public class HurtableRuntimeSet : RuntimeSet<IHurtable> {}
}
