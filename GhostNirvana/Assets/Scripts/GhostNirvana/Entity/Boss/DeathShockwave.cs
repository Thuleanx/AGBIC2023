using UnityEngine;
using System.Collections.Generic;
using CombatSystem;

namespace GhostNirvana {

public class DeathShockwave : MonoBehaviour {
    [SerializeField] float waveSpeed;
    [SerializeField] float waveAcceleration;
    [SerializeField] MovableAgentRuntimeSet allEnemies;
    float startTime;

    void OnEnable() => startTime = Time.time;

    void Update() {
        if (!allEnemies) return;

        float timeSinceStart = Time.time - startTime;
        float waveDistance = waveSpeed * timeSinceStart + 
            waveAcceleration * timeSinceStart * timeSinceStart / 2;

        List<Status> enemiesToKill = new List<Status>();

        // this can lag, but hopefully ok
        foreach (MovableAgent movableAgent in allEnemies) {
            Vector3 agentPos = movableAgent.transform.position;
            float distanceToWaveCenterSquared = Vector3.SqrMagnitude(agentPos - transform.position);

            // if not reached yet, continue
            if (distanceToWaveCenterSquared > waveDistance * waveDistance) continue;

            Status status = (movableAgent as MonoBehaviour)
                ?.GetComponent<Status>();
            if (!status) continue;

            enemiesToKill.Add(status);
        }

        foreach (Status enemyStatus in enemiesToKill)
            enemyStatus?.Kill();
    }
}

}
