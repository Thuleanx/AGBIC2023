using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GhostNirvana {

[RequireComponent(typeof(Appliance))]
public class ApplianceAnimator : MonoBehaviour {
    [field: SerializeField]
    public Animator Anim { get; private set; }
    [SerializeField] Material emissiveMaterial;
    [SerializeField] ParticleSystem possessionImmuneVFX;

    Appliance Appliance;

    enum AnimationState {
        Normal = 0,
        Possessed = 1
    }

    List<KeyValuePair<Renderer, int>> meshRenderersWithEmission = new List<KeyValuePair<Renderer, int>>();
    AnimationState currentState = AnimationState.Normal;

    void Awake() {
        Appliance = GetComponent<Appliance>();

        foreach (Renderer renderer in Anim.GetComponentsInChildren<Renderer>()) {
            for (int i = 0; i < renderer.sharedMaterials.Length; i++) {
                Material mat = renderer.sharedMaterials[i];
                bool hasEmissiveOn = mat == emissiveMaterial;
                if (hasEmissiveOn) meshRenderersWithEmission.Add(new KeyValuePair<Renderer, int>(renderer, i));
            }
        }
    }

    void OnEnable() {
        ToggleEmission(on: false);
    }

    protected void Update() {
        switch (currentState) {
            case AnimationState.Normal:
                ToggleEmission(on: false);
                if (Appliance.IsPossessedByGhost) currentState = AnimationState.Possessed;
                break;
            case AnimationState.Possessed:
                ToggleEmission(on: true);
                if (!Appliance.IsPossessedByGhost) currentState = AnimationState.Normal;
                break;
        }
    }

    void ToggleEmission(bool on) {
        foreach (var (renderer, materialIndex) in meshRenderersWithEmission) {
            renderer.materials[materialIndex].globalIlluminationFlags = on ? MaterialGlobalIlluminationFlags.None : MaterialGlobalIlluminationFlags.EmissiveIsBlack;

            if (on) renderer.materials[materialIndex].EnableKeyword("_EMISSION");
            else renderer.materials[materialIndex].DisableKeyword("_EMISSION");
        }
    }

    protected void LateUpdate() {
        if (!Appliance) return; // this is impossible unless project configured wrong

        Anim?.SetInteger("State", (int) currentState);

        bool shouldShimmer = !Appliance.IsPossessedByGhost && !Appliance.IsBeingPossessed && Appliance.PossessionImmune;
        if (possessionImmuneVFX.isPlaying ^ shouldShimmer) {
            if (shouldShimmer) possessionImmuneVFX?.Play();
            else possessionImmuneVFX?.Stop();
        }
    }
}

}

