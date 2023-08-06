using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    [SerializeField] float reloadAnimationDuration;

    Miyu Miyu;

    enum AnimationState {
        Normal = 0,
        Reload = 1,
        Dead = 2
    }

    AnimationState currentState;

    void Awake() {
        Miyu = GetComponent<Miyu>();
    }

    protected void Update() {
        switch (currentState) {
            case AnimationState.Normal:
                if (Miyu.IsDead) currentState = AnimationState.Dead;
                if (!Miyu.HasBullet) currentState = AnimationState.Reload;
                break;
            case AnimationState.Reload:
                if (Miyu.IsDead) currentState = AnimationState.Dead;
                if (Miyu.HasBullet) currentState = AnimationState.Normal;
                break;
            case AnimationState.Dead:
                break;
        }
    }

    protected void LateUpdate() {
        if (!Miyu) return; // this is impossible unless project configured wrong

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
}

}

