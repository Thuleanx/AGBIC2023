using UnityEngine;

namespace CombatSystem {

[CreateAssetMenu(fileName = "Data", 
        menuName = "~/CombatSystem/BaseStats", order = 1)]
public class BaseStats : ScriptableObject {
    [field:SerializeField]
    public int MaxHealth { get; protected set; }
    [field:SerializeField]
    public int Damage { get; protected set; }
    [field:SerializeField]
    public float MovementSpeed {get; protected set; }
    [field:SerializeField]
    public float AccelerationAlpha {get; protected set; }
    [field:SerializeField]
    public float DeccelerationAlpha {get; protected set; }
    [field:SerializeField]
    public float Knockback {get; protected set; }
}

}
