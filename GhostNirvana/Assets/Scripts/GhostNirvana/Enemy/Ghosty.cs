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
    [field:SerializeField]
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

    void Awake() {
        Controller = GetComponent<CharacterController>();
        Status.Owner = this;
    }

    void OnEnable() {
        Status.OnDeath.AddListener(OnDeath);
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

        Vector3 desiredVelocity = input.desiredMovement * Status.BaseStats.MovementSpeed;

        Velocity = Mathx.Damp(Vector3.Lerp, Velocity, desiredVelocity,
                              (Velocity.sqrMagnitude > desiredVelocity.sqrMagnitude)
                              ? Status.BaseStats.DeccelerationAlpha : Status.BaseStats.AccelerationAlpha,
                              Time.deltaTime);

        Controller.Move(Velocity * Time.deltaTime);
    }

    void IHurtable.OnTakeDamage(float damageAmount, DamageType damageType, Hit hit)
        => Status.TakeDamage(damageAmount);

    bool IHurtResponder.ValidateHit(Hit hit) => true;

    void IHurtResponder.RespondToHit(Hit hit) {
    }
    void OnDeath() {
        Status.OnDeath.RemoveListener(OnDeath);
        Dispose();
    }

    protected override IEnumerator IDispose() {
        // actually dispose the thing
        yield return base.IDispose();
    }
}

}
