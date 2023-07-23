using System;
using Control;
using CombatSystem;
using Optimization;
using UnityEngine;
using Utils;
using NaughtyAttributes;

namespace GhostNirvana {

[RequireComponent(typeof(CharacterController))]
public abstract class PossessableAgent<Input> : PoolableEntity, IDoll<Input>, IKnockbackable {

#region Components
    public CharacterController Controller { get; private set; }
#endregion

    public Vector3 Velocity { get; protected set; }
    public Vector3 Knockback { get; protected set; }

    protected IPossessor<Input> _possessor;
    IPossessor<Input> IDoll<Input>.Possessor {
        get => _possessor;
        set => _possessor = value;
    }
    public Input input {get; protected set;}

    [BoxGroup("Movement"), SerializeField] float knockbackResistance = 8;
    float knockbackStrength = 0;

    protected virtual void Awake() {
        Controller = GetComponent<CharacterController>();
    }

    protected void PerformUpdate(Action Update) {
        if (_possessor != null) input = _possessor.GetCommand();

        Update();

        if (knockbackStrength < 0.1) knockbackStrength = 0;
        else knockbackStrength = Mathx.Damp(Mathf.Lerp, knockbackStrength, 0, knockbackResistance, Time.deltaTime);

        Controller.Move(TrueVelocity * Time.deltaTime);
    }

    public Vector3 TrueVelocity {
        get => Vector3.Lerp(Velocity, Knockback, knockbackStrength);
    }

    void IKnockbackable.OnKnockback(float amount, Vector3 dir, Hit hit) {
        if (Knockback.sqrMagnitude < amount * amount)
            Knockback = dir * amount;
        knockbackStrength = 1;
    }
}

}
