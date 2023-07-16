using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using Optimization;
using Control;
using Danmaku;
using CombatSystem;
using Thuleanx.Utils;
using Base;

namespace GhostNirvana {

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Status))]
public partial class Ghosty : PoolableEntity, IDoll<Ghosty.Input>, IHurtable, IHurtResponder {

    Entity IHurtResponder.Owner => this;
    IPossessor<Input> _possessor;
    IPossessor<Input> IDoll<Input>.Possessor {
        get => _possessor;
        set => _possessor = value;
    }

    public enum States {
        Seek,
        Death
    }

    #region Components
    public CharacterController Controller {
        get;
        private set;
    }
    public Status Status {
        get;
        private set;
    }
    #endregion

    public struct Input {
        public Vector3 desiredMovement;
    }

    Input input;
    public Vector3 Velocity {
        get;
        private set;
    }

    [SerializeField] float movementSpeed;
    [SerializeField] float accelerationAlpha;
    [SerializeField] float deccelerationAlpha;

    void Awake() {
        Controller = GetComponent<CharacterController>();
        Status = GetComponent<Status>();

        foreach (Hurtbox hurtbox in GetComponentsInChildren<Hurtbox>())
            hurtbox.HurtResponder = this;
    }

    void Start() {
        HealthBarManager.Instance.AddStatus(Status);
    }

    void OnDisable() {
        HealthBarManager.Instance.RemoveStatus(Status);
    }

    void Update() {
        if (_possessor != null) input = _possessor.GetCommand();

        Vector3 desiredVelocity = input.desiredMovement * movementSpeed;

        Velocity = Mathx.Damp(Vector3.Lerp, Velocity, desiredVelocity,
                              (Velocity.sqrMagnitude > desiredVelocity.sqrMagnitude)
                              ? deccelerationAlpha : accelerationAlpha,
                              Time.deltaTime);

        Controller.Move(Velocity * Time.deltaTime);
    }

    void IHurtable.OnTakeDamage(float damageAmount, DamageType damageType, Hit hit)
        => Status.TakeDamage(damageAmount);

    bool IHurtResponder.ValidateHit(Hit hit) => true;

    void IHurtResponder.RespondToHit(Hit hit) {
    }
}

}
