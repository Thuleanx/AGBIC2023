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

    protected void Update() {
        if (Miyu && Time.timeScale > 0) {
            Vector3 aimPos = Miyu.input.targetPositionWS;
            transform.position = aimPos;
        }
    }
}

}

