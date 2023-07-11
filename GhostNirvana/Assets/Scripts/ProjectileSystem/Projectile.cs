using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Base;
using Optimization;

namespace Danmaku {

public class Projectile : PoolableEntity {
    Entity owner;

    new Rigidbody rigidbody;

    void Awake() {
        rigidbody = GetComponent<Rigidbody>();
    }

    public void Initialize(Vector3 velocity, bool faceDirection = true) {
        rigidbody.velocity = velocity;
    }
}

}
