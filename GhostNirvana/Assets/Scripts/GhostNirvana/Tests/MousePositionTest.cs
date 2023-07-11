using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GhostNirvana.Test {

public class MousePositionTest : MonoBehaviour {
    public Miyu Miyu { get; private set; }

    void Awake() {
        Miyu = GetComponentInParent<Miyu>();
    }

    void Update() {
        if (Miyu)
            transform.position = Miyu.input.targetPositionWS;
    }
}

}
