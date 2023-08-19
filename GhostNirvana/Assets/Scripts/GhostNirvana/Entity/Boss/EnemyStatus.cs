using UnityEngine;
using CombatSystem;

namespace GhostNirvana {

[RequireComponent(typeof(Status))]
public class EnemyStatus : MonoBehaviour {
    [SerializeField] StatusRuntimeSet allEnemyStatus;
    Status status;

    protected void Awake() {
        status = GetComponent<Status>();
    }

    protected void OnEnable() => allEnemyStatus.Add(status);
    protected void OnDisable() => allEnemyStatus.Remove(status);
}

}
