using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using NaughtyAttributes;
using ScriptableBehaviour;

namespace GhostNirvana {

[RequireComponent(typeof(Miyu))]
public class MiyuAnimator : MonoBehaviour {
    [field: SerializeField]
    public Animator Anim { get; private set; }
    [SerializeField] Transform aimingTarget;
    [SerializeField] MultiAimConstraint aimConstraint;

    [SerializeField, AnimatorParam("Anim")] string param_LocomotionAnimationSpeed;
    [SerializeField, AnimatorParam("Anim")] string param_Speed;
    [SerializeField, AnimatorParam("Anim")] string param_State;

    [SerializeField] LinearFloat playerSpeed;

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

        Anim?.SetFloat(param_Speed, Miyu.Velocity.magnitude);
        Anim?.SetInteger(param_State, (int) currentState);

        if (aimingTarget && Time.timeScale > 0) aimingTarget.transform.position = Miyu.input.targetPositionWS;
    }
}

}

