using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Optimization;
using NaughtyAttributes;
using Control;
using Danmaku;

namespace GhostNirvana {

public partial class Miyu : PossessableAgent<Miyu.Input> {
    public static Miyu Instance;

    public enum States {
        Grounded,
        Dash,
    }

    #region Components
    public MiyuStateMachine StateMachine { get; private set; }
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

    protected override void Awake() {
		base.Awake();
        StateMachine = GetComponent<MiyuStateMachine>();
        Instance = this;

        health.Value = health.Limiter;
    }

    protected void Update() {
        PerformUpdate(StateMachine.RunUpdate);
    }

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
