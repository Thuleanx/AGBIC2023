using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Optimization;
using NaughtyAttributes;
using Control;

namespace GhostNirvana {

public partial class Miyu : PoolableEntity, IDoll<Miyu.ControllerInput> {
    public enum States {
        Grounded,
        Dash,
    }

#region Components
    public CharacterController Controller { get; private set; }
    public MiyuStateMachine StateMachine { get; private set; }
#endregion

#region Movement
    [HorizontalLine(color:EColor.Blue)]
    [BoxGroup("Movement"), Range(0, 10), SerializeField] float movementSpeed = 4;
    [BoxGroup("Movement"), Range(0, 64), SerializeField] float accelerationAlpha = 24;
    [BoxGroup("Movement"), Range(0, 64), SerializeField] float deccelerationAlpha = 12;
    [BoxGroup("Movement"), Range(0, 720), SerializeField] float turnSpeed = 24;

    protected Vector3 Velocity;
#endregion

    ControllerInput Input;

#region Doll Interface Implementation
    IDoll<ControllerInput> MiyuAsDoll => this;

    IPossessor<ControllerInput> _possessor;
    IPossessor<ControllerInput> IDoll<ControllerInput>.Possessor {
        get => _possessor;
        set => _possessor = value;
    }
#endregion

    void Awake() {
        Controller = GetComponent<CharacterController>();
        StateMachine = GetComponent<MiyuStateMachine>();
    }

    void Update() {
        // grab input from possessor
        if (MiyuAsDoll.IsPossessed)
            Input = MiyuAsDoll.GetCommand();

        StateMachine?.RunUpdate();

        Move(Velocity * Time.deltaTime);
    }

    void Move(Vector3 displacement) => Controller?.Move(displacement);

    public void TurnToFace(Vector3 dir, float turnSpeed) {
        dir.y = 0;
        if (dir != Vector3.zero) {
            Quaternion desiredRotation = Quaternion.LookRotation(dir, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, desiredRotation, Time.deltaTime * turnSpeed);
        }
    }
}

}
