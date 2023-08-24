using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

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
    [SerializeField] List<ParticleSystem> knockedOutSystems;
    [SerializeField] float knockedOutVFXDuration;
    Timer beingKnockedOutOfAppliance;

    void Awake() {
        Ghosty = GetComponent<Ghosty>();
    }

    void OnEnable() {
        currentState = AnimationState.Normal;
    }

    public void OnKnockOutOfAppliance() {
        beingKnockedOutOfAppliance = knockedOutVFXDuration;
    }

    protected void Update() {
        if (!Ghosty) return;
        currentState = !Ghosty.IsPossessing ? AnimationState.Normal : AnimationState.Possessing;
    }

    protected void LateUpdate() {
        if (!Ghosty) return; // this is impossible unless project configured wrong
        Anim?.SetInteger("State", (int) currentState);
        foreach (ParticleSystem system in knockedOutSystems) {
            if (system.isPlaying ^ beingKnockedOutOfAppliance) {
                if (system.isPlaying)   system.Play();
                else                    system.Stop();
            }
        }
    }
}

}

