using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Optimization;
using NaughtyAttributes;
using Control;
using Danmaku;

namespace GhostNirvana {

public partial class Miyu : PoolableEntity, IDoll<Miyu.Input> {
    public static Miyu Instance;

    public enum States {
        Grounded,
        Dash,
    }

    #region Components
    public CharacterController Controller {
        get;
        private set;
    }
    public MiyuStateMachine StateMachine {
        get;
        private set;
    }
    #endregion

    #region Progression
    [HorizontalLine(color:EColor.Blue)]
    [BoxGroup("Progression"), SerializeField, Expandable] LinearLimiterFloat xp;
    #endregion

    #region Movement
    [HorizontalLine(color:EColor.Blue)]
    [BoxGroup("Movement"), SerializeField, Expandable] LinearFloat movementSpeed;
    [BoxGroup("Movement"), Range(0, 64), SerializeField] float accelerationAlpha = 24;
    [BoxGroup("Movement"), Range(0, 64), SerializeField] float deccelerationAlpha = 12;
    [BoxGroup("Movement"), Range(0, 720), SerializeField] float turnSpeed = 24;

    protected Vector3 Velocity;
    #endregion

    #region Combat
    [HorizontalLine(color:EColor.Blue)]
    [BoxGroup("Combat"), SerializeField, Required, ShowAssetPreview]
    Projectile projectilePrefab;
    [BoxGroup("Combat"), SerializeField, Expandable] LinearLimiterFloat health;
    [BoxGroup("Combat"), SerializeField, Expandable] LinearFloat attackSpeed;
    [BoxGroup("Combat"), SerializeField, Expandable] LinearFloat bulletSpeed;
    [BoxGroup("Combat"), SerializeField, Expandable] LinearLimiterFloat magazine;
    [BoxGroup("Combat"), SerializeField, Expandable] LinearFloat reloadRate;
    #endregion


    #region Doll Interface Implementation
    IDoll<Input> MiyuAsDoll => this;

    IPossessor<Input> _possessor;
    IPossessor<Input> IDoll<Input>.Possessor {
        get => _possessor;
        set => _possessor = value;
    }
    #endregion

    public Input input { get; private set; }

    void Awake() {
        Controller = GetComponent<CharacterController>();
        StateMachine = GetComponent<MiyuStateMachine>();
        Instance = this;

        health.Value = health.Limiter;
    }

    void Update() {
        // grab input from possessor
        if (MiyuAsDoll.IsPossessed)
            input = MiyuAsDoll.GetCommand();

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

    public void ShootProjectile(Vector3 targetDirection) {
        Projectile bullet = ObjectPoolManager.Instance.Borrow(gameObject.scene,
                projectilePrefab, transform.position, transform.rotation);

        bullet.Initialize(targetDirection * bulletSpeed.Value);
    }
}

}
