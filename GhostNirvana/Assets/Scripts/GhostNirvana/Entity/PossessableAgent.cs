using System;
using Control;
using CombatSystem;
using Optimization;
using UnityEngine;
using Utils;
using NaughtyAttributes;

namespace GhostNirvana {

[RequireComponent(typeof(CharacterController))]
public abstract class PossessableAgent<Input> : MovableAgent, IDoll<Input> {

    protected IPossessor<Input> _possessor;
    IPossessor<Input> IDoll<Input>.Possessor {
        get => _possessor;
        set => _possessor = value;
    }

    public bool IsPossessed => _possessor != null;

    public Input input {get; protected set;}

    protected override void PerformUpdate(Action Update) {
        if (_possessor != null) input = _possessor.GetCommand();
        base.PerformUpdate(Update);
    }
}

}
