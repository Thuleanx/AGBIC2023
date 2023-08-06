using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GhostNirvana {

[RequireComponent(typeof(Ghosty))]
public class GhostyAnimator : MonoBehaviour {
    [field: SerializeField] public Animator Anim { get; private set; }

    Ghosty Ghosty;
    enum AnimationState {
        Normal = 0,
        Possessing = 1
    }

    [SerializeField] AnimationState currentState = AnimationState.Normal;

    void Awake() {
        Ghosty = GetComponent<Ghosty>();
    }

    void OnEnable() {
        currentState = AnimationState.Normal;
    }

    protected void Update() {
        if (!Ghosty) return;
        currentState = !Ghosty.IsPossessing ? AnimationState.Normal : AnimationState.Possessing;
    }

    protected void LateUpdate() {
        if (!Ghosty) return; // this is impossible unless project configured wrong
        Anim?.SetInteger("State", (int) currentState);
    }
}

}

