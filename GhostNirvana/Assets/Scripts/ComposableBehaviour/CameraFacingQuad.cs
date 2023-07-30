using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ComposableBehaviour {

public class CameraFacingQuad : MonoBehaviour
{
    new Camera camera;

    public void Awake() {
        camera = Camera.main;
    }

    void LateUpdate() {
        if (camera.orthographic) {
            transform.forward = camera.transform.forward;
        } else {
            transform.LookAt(transform.position - camera.transform.forward, camera.transform.up);
        }
    }
}

}
