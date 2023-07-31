using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GhostNirvana {

[RequireComponent(typeof(Appliance))]
public class ApplianceAnimator : MonoBehaviour {
    [field: SerializeField]
    public Animator Anim { get; private set; }
    Appliance Appliance;

    enum AnimationState {
        Normal = 0,
        Possessed = 1
    }

    AnimationState currentState;

    void Awake() {
        Appliance = GetComponent<Appliance>();
    }

    protected void Update() {
        switch (currentState) {
            case AnimationState.Normal:
                if (Appliance.IsPossessed)
                    currentState = AnimationState.Possessed;
                break;
            case AnimationState.Possessed:
                if (!Appliance.IsPossessed)
                    currentState = AnimationState.Normal;
                break;
        }
    }

    protected void LateUpdate() {
        if (!Appliance) return; // this is impossible unless project configured wrong

        Anim?.SetInteger("State", (int) currentState);
    }
}

}

