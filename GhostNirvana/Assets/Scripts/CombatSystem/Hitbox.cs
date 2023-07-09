using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CombatSystem {

[RequireComponent(typeof(Collider))]
public class Hitbox : MonoBehaviour, IHitbox {
    [SerializeField] BoxCollider _collider;
    [SerializeField] LayerMask _hurtboxMask;

    float _thickness = 0.025f;
    IHitResponder _hitResponder;

    public IHitResponder HitResponder {
        get => _hitResponder;
        set => _hitResponder = value;
    }

    public void CheckForHits() {
        Vector3 scaledSize = new Vector3(
            _collider.size.x * transform.lossyScale.x,
            _collider.size.y * transform.lossyScale.y,
            _collider.size.z * transform.lossyScale.z
        );

        float distance = scaledSize.y - _thickness;
        Vector3 direction = transform.up;
        Vector3 center = transform.TransformPoint(_collider.center);
        Vector3 start = center - direction * distance / 2;
        Vector3 halfExtents = new Vector3(
                scaledSize.x, _thickness, scaledSize.z) / 2;
        Quaternion orientation = transform.rotation;

        RaycastHit[] hits = Physics.BoxCastAll(
            start, halfExtents, direction, orientation, distance, _hurtboxMask);

        foreach (RaycastHit hit in hits) {
            IHurtbox hurtbox = hit.collider.GetComponent<IHurtbox>();
            if (hurtbox == null || hurtbox.Active) 
                continue;

            Hit hitData = new Hit(
                hit.point == Vector3.zero ? center : hit.point,
                hit.normal,
                Time.time,
                this,
                hurtbox
            );

            // validate the hit
            // we dont validate for the hitbox, because it's assumed that 
            // the hitbox can validate the hit itself before this point
            bool hitValid = 
                hurtbox.HurtResponder != null && 
                _hitResponder != null &&
                hurtbox.ValidateHit(hitData) && 
                hurtbox.HurtResponder.ValidateHit(hitData) &&
                _hitResponder.ValidateHit(hitData);

            if (!hitValid) continue;

            hurtbox.HurtResponder?.RespondToHit(hitData);
            _hitResponder?.RespondToHit(hitData);
        }
    }
}

}
