using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using NaughtyAttributes;
using ScriptableBehaviour;
using Utils;

namespace GhostNirvana {

public class MiyuAimTarget : MonoBehaviour {
    [SerializeField] Miyu Miyu;
    [SerializeField] float dampFactor = 16f;

    protected void Update() {
        if (Miyu && Miyu.IsPossessed && Time.timeScale > 0) {
            Vector3 aimPos = Miyu.input.targetPositionWS;
            transform.position = Mathx.Damp(
                Vector3.Lerp,
                transform.position,
                aimPos,
                dampFactor,
                Time.deltaTime);
        }
    }
}

}

