using UnityEngine;

namespace CombatSystem {

[CreateAssetMenu(fileName = "Data", 
        menuName = "~/CombatSystem/BaseStats", order = 1)]
public class BaseStats : ScriptableObject {
    [field:SerializeField]
    public int MaxHealth { get; set; }
    [field:SerializeField]
    public int Damage { get; set; }
    [field:SerializeField]
    public float MovementSpeed {get; set; }
    [field:SerializeField]
    public float Acceleration {get; set; }
    [field:SerializeField]
    public float Knockback {get; set; }
}

}
