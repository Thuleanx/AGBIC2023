using UnityEngine;

namespace CombatSystem {

[CreateAssetMenu(fileName = "Data", 
        menuName = "~/CombatSystem/BaseStats", order = 1)]
public class BaseStats : ScriptableObject {
    [field:SerializeField]
    public float MaxHealth { get; protected set; }
    [field:SerializeField]
    public float Damage { get; protected set; }
    [field:SerializeField]
    public float MovementSpeed {get; protected set; }
    [field:SerializeField]
    public float AccelerationAlpha {get; protected set; }
    [field:SerializeField]
    public float DeccelerationAlpha {get; protected set; }
}

}
