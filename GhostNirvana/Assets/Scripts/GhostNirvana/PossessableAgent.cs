using System;
using Control;
using Optimization;
using UnityEngine;

namespace GhostNirvana {

[RequireComponent(typeof(CharacterController))]
public abstract class PossessableAgent<Input> : PoolableEntity, IDoll<Input> {

#region Components
    public CharacterController Controller { get; private set; }
#endregion

    public Vector3 Velocity { get; protected set; }

    protected IPossessor<Input> _possessor;
    IPossessor<Input> IDoll<Input>.Possessor {
        get => _possessor;
        set => _possessor = value;
    }
    public Input input {get; protected set;}

    protected virtual void Awake() {
        Controller = GetComponent<CharacterController>();
    }

    protected void PerformUpdate(Action Update) {
        if (_possessor != null) input = _possessor.GetCommand();

        Update();

        Controller.Move(Velocity * Time.deltaTime);
    }
}

}
