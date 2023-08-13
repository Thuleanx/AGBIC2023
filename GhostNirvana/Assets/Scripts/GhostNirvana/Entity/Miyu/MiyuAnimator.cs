using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Animations.Rigging;
using NaughtyAttributes;
using ScriptableBehaviour;
using Utils;

namespace GhostNirvana {

[RequireComponent(typeof(Miyu))]
public class MiyuAnimator : MonoBehaviour {
    [field: SerializeField]
    public Animator Anim { get; private set; }
    [SerializeField] MultiAimConstraint aimConstraint;

    [SerializeField, AnimatorParam("Anim")] string param_LocomotionAnimationSpeed;
    [SerializeField, AnimatorParam("Anim")] string param_ReloadAnimationSpeed;
    [SerializeField, AnimatorParam("Anim")] string param_Speed;
    [SerializeField, AnimatorParam("Anim")] string param_State;

    [SerializeField] LinearFloat playerSpeed;
    [SerializeField] LinearFloat reloadSpeed;
    [SerializeField] LinearFloat shield;

    [SerializeField] float reloadAnimationDuration;
    [SerializeField] float shootAnimationDuration;
    [SerializeField] ParticleSystem shieldBreak;
    [SerializeField] ParticleSystem shieldCharge;
    [SerializeField] GameObject shieldOn;
    [SerializeField] UnityEvent OnShieldActivation;

    Miyu Miyu;
    Timer shooting;
    float shieldLastFrame;

    enum AnimationState {
        Normal = 0,
        Reload = 1,
        Dead = 2,
        Shoot = 3
    }

    AnimationState currentState;

    void Awake() {
        Miyu = GetComponent<Miyu>();
    }

    void OnEnable() => Miyu.OnShootEvent.AddListener(OnShoot);
    void OnDisable() => Miyu.OnShootEvent.RemoveListener(OnShoot);

    protected void Update() {
        if (Miyu.IsDead) currentState = AnimationState.Dead;

        switch (currentState) {
            case AnimationState.Normal:
                if (!Miyu.HasBullet) currentState = AnimationState.Reload;
                else if (shooting) currentState = AnimationState.Shoot;
                break;
            case AnimationState.Reload:
                if (Miyu.HasBullet) currentState = AnimationState.Normal;
                break;
            case AnimationState.Shoot:
                if (!Miyu.HasBullet)    currentState = AnimationState.Reload;
                else if (!shooting)     currentState = AnimationState.Normal;
                break;
            case AnimationState.Dead:
                break;
        }
    }

    void OnShoot() {
        shooting = shootAnimationDuration;
    }

    protected void LateUpdate() {
        if (!Miyu) return; // this is impossible unless project configured wrong

        HandleShieldState();

        if (Miyu.Velocity.magnitude < 0)    Anim?.SetFloat(param_LocomotionAnimationSpeed, 1);
        else                                Anim?.SetFloat(param_LocomotionAnimationSpeed, Mathf.Max(Miyu.Velocity.magnitude / playerSpeed.BaseValue, 1));

        float relativeReloadSpeed = reloadSpeed.Value / reloadAnimationDuration;

        Anim?.SetFloat(param_ReloadAnimationSpeed, relativeReloadSpeed);
        Anim?.SetInteger(param_State, (int) currentState);

        if (Miyu.IsDead) {
            aimConstraint.weight = 0;
            return;
        }

        Vector3 aimPos = Miyu.input.targetPositionWS;
        Vector3 relativeAimPos = aimPos - Miyu.transform.position;
        relativeAimPos.y = 0;

        float speedParam = Miyu.Velocity.magnitude;
        Vector3 directionToFace = Miyu.Velocity;
        if (directionToFace.sqrMagnitude < 0.01f) {
            directionToFace = relativeAimPos;
            directionToFace.Normalize();
        }

        bool aimOppositeOfVelocity = Vector3.Dot(directionToFace, relativeAimPos) < 0;

        if (aimOppositeOfVelocity) {
            directionToFace *= -1;
            speedParam *= -1;
        }

        Anim?.SetFloat(param_Speed, speedParam);
        Miyu.TurnToFace(directionToFace, Miyu.TurnSpeed);
    }

    void HandleShieldState() {
        if (shield.Value == 0) return;

        if (shield.Value < 1) {
            shieldOn.gameObject.SetActive(false);
            if (shield.Value >= 0.95f && !shieldCharge.isPlaying)
                shieldCharge.Play();
        } else if (shield.Value >= 1) {
            if (!shieldOn.gameObject.activeSelf) OnShieldActivation?.Invoke();
            shieldOn.gameObject.SetActive(true);
            if (shieldCharge.isPlaying) shieldCharge.Stop();
        }

        if (shieldLastFrame >= 1 && shield.Value < 1) {
            shieldBreak.Stop();
            shieldBreak.Play();
        }
        shieldLastFrame = shield.Value;
    }
}

}

