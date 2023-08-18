using Base;
using UnityEngine;
using UnityEngine.Events;
using CombatSystem;
using NaughtyAttributes;
using Optimization;
using Utils;
using DG.Tweening;

namespace GhostNirvana {

public partial class ApplianceMass : PossessableAgent<ApplianceMass.Input> {
    public struct Input {
        public Vector3 desiredMovement;
    }

    float acceleration;
    float movementSpeed;
    float maxSpeed;

    protected override void Awake() {
        base.Awake();
    }

    protected override void OnEnable() {
        base.OnEnable();
    }

    protected void Update() => PerformUpdate(InnerUpdate);

    void InnerUpdate() {

        Vector3 desiredVelocity = input.desiredMovement;

        Velocity = Mathx.AccelerateTowards(
            currentVelocity: Velocity,
            desiredVelocity,
            acceleration,
            maxSpeed,
            Time.deltaTime
        );
    }
}

}
