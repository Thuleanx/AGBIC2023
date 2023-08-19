using Optimization;
using Base;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Events;
using System.Collections.Generic;
using Utils;
using ComposableBehaviour;
using CombatSystem;

namespace GhostNirvana {

public class RotatingMass : PoolableEntity {
    [SerializeField] List<PoolableEntity> rotatorTemplates;
    [SerializeField] int rotatorsToSpawn;
    [SerializeField] int rotatorsAtDeath;
    [SerializeField] UnityEvent onDeath;
    [SerializeField] Vector3 centerOffset;

    List<PoolableEntity> rotators = new List<PoolableEntity>();
    int rotatorCount => rotators.Count;

    bool isDead => rotators.Count == rotatorsAtDeath;

    protected override void OnEnable() {
        base.OnEnable();

        SpawnRotators();
    }

    protected void OnDisable() {
    }

    void SpawnRotators() {
        DisposeRotators();
        rotators = new List<PoolableEntity>();
        while (rotatorCount < rotatorsToSpawn) {
            int randomRotatorIndex = Mathx.RandomRange(0, rotatorTemplates.Count);
            PoolableEntity rotator = ObjectPoolManager.Instance.Borrow(
                App.GetActiveScene(),
                rotatorTemplates[randomRotatorIndex],
                transform.position
            );
            Wander wanderer = rotator.GetComponent<Wander>();
            wanderer.AttachedTransform = transform;
            wanderer.Offset = centerOffset;

            Status status = rotator.GetComponent<Status>();
            status.OnDeath.AddListener(OnRotatorDeath);

            rotators.Add(rotator);

            rotator.transform.SetParent(transform);
        }
    }

    void OnRotatorDeath(Status status) {
        DetachStatus(status);
        rotators.Remove(status.GetComponent<PoolableEntity>());
        if (rotators.Count <= rotatorsAtDeath) OnDeath();
    }

    void OnDeath() {
        foreach (PoolableEntity rotator in rotators) {
            Status rotatorStatus = rotator.GetComponent<Status>();
            DetachStatus(rotatorStatus);
            int health = rotatorStatus.Health;
            rotatorStatus.TakeDamage(health); // kills remaining rotators, making them drop to the ground
        }
        rotators.Clear();
        onDeath?.Invoke();
        this.Dispose();
    }

    void DetachStatus(Status status) {
        status.OnDeath.RemoveListener(OnRotatorDeath);
        status.transform.SetParent(null);
        DontDestroyOnLoad(status.gameObject);
    }

    void DisposeRotators() {
        if (rotators != null) return;
        foreach (PoolableEntity rotator in rotators)
            rotator.Dispose();
        rotators.Clear();
    }
}

}
