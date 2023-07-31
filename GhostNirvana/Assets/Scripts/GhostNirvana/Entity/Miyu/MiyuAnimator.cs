using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GhostNirvana {

[RequireComponent(typeof(Miyu))]
public class MiyuAnimator : MonoBehaviour {
    [field: SerializeField]
    public Animator Anim { get; private set; }
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

        Anim?.SetFloat("Speed", Miyu.Velocity.magnitude);
        Anim?.SetInteger("State", (int) currentState);
    }
}

}

