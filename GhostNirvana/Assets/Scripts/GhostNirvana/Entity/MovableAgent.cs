using System;
using CombatSystem;
using Optimization;
using UnityEngine;
using Utils;
using NaughtyAttributes;

namespace GhostNirvana {

[RequireComponent(typeof(CharacterController))]
public abstract class MovableAgent : PoolableEntity, IKnockbackable {

#region Components
    public CharacterController Controller { get; private set; }
#endregion

    public Vector3 Velocity { get; protected set; }
    public Vector3 Knockback { get; protected set; }

    [BoxGroup("Movement"), SerializeField, HideIf("knockbackImmune")] float knockbackResistance = 8;
    [BoxGroup("Movement"), SerializeField] bool knockbackImmune = false;
    float knockbackStrength = 0;


    protected virtual void Awake() {
        Controller = GetComponent<CharacterController>();
    }

    protected override void OnEnable() {
        base.OnEnable();
        knockbackStrength = 0;
        Knockback = Vector3.zero;
    }

    protected virtual void PerformUpdate(Action Update) {
        Update();

        if (knockbackStrength < 0.1) {
            knockbackStrength = 0;
            Knockback = Vector3.zero;
        } else
            knockbackStrength = Mathx.Damp(Mathf.Lerp, knockbackStrength, 0, knockbackResistance, Time.deltaTime);

        Controller.Move(TrueVelocity * Time.deltaTime);
    }

	[SerializeField, ReadOnly]
    public bool FreezePosition = false;

    public Vector3 TrueVelocity {
        get => FreezePosition ? Vector3.zero : Vector3.Lerp(Velocity, Knockback, knockbackStrength);
    }

    void IKnockbackable.OnKnockback(float amount, Vector3 dir) {
        if (knockbackImmune) return;
        bool knockbackStrongerThanCurrent = Knockback.sqrMagnitude * knockbackStrength * knockbackStrength < amount * amount;
        if (knockbackStrongerThanCurrent) {
            Knockback = dir * amount;
            knockbackStrength = 1;
        }
    }

    public void TurnToFace(Vector3 dir, float turnSpeed) {
        dir.y = 0;
        if (dir != Vector3.zero) {
            Quaternion desiredRotation = Quaternion.LookRotation(dir, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, desiredRotation, Time.deltaTime * turnSpeed);
        }
    }
}


}
