using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CombatSystem {

[RequireComponent(typeof(Collider))]
public class Hitbox : MonoBehaviour, IHitbox {
    [SerializeField] Collider _collider;
    [SerializeField] LayerMask _hurtboxMask;
    Rigidbody _rigidbody;
    [SerializeField] bool hitNormalIsVelocityDirection;

    float _thickness = 0.025f;
    IHitResponder _hitResponder;

    public IHitResponder HitResponder {
        get => _hitResponder;
        set => _hitResponder = value;
    }

    void Awake() {
        _collider = GetComponent<Collider>();
        _rigidbody = GetComponentInParent<Rigidbody>();
    }

    public void CheckForHits() {
        void ValidateAndSendHit(IHurtbox hurtbox, Hit hitData) {
            // validate the hit
            // we dont validate for the hitbox, because it's assumed that
            // the hitbox can validate the hit itself before this point
            bool hitValid =
                hurtbox.ValidateHit(hitData) &&
                hurtbox.HurtResponder != null &&
                hurtbox.HurtResponder.ValidateHit(hitData) &&
                (_hitResponder == null || _hitResponder.ValidateHit(hitData));

            if (!hitValid) return;

            hurtbox.HurtResponder?.RespondToHurt(hitData);
            _hitResponder?.RespondToHit(hitData);
        }

        if (_collider is BoxCollider) {
            BoxCollider boxCollider = _collider as BoxCollider;
            Vector3 scaledSize = new Vector3(
                boxCollider.size.x * transform.lossyScale.x,
                boxCollider.size.y * transform.lossyScale.y,
                boxCollider.size.z * transform.lossyScale.z
            );

            float distance = scaledSize.y - _thickness;
            Vector3 direction = transform.up;
            Vector3 center = transform.TransformPoint(boxCollider.center);
            Vector3 start = center - direction * distance / 2;
            Vector3 halfExtents = new Vector3(
                scaledSize.x, _thickness, scaledSize.z) / 2;
            Quaternion orientation = transform.rotation;

            RaycastHit[] hits = Physics.BoxCastAll(start, halfExtents, direction, orientation, distance, _hurtboxMask, QueryTriggerInteraction.Collide);

            foreach (RaycastHit hit in hits) {
                IHurtbox hurtbox = hit.collider.GetComponent<IHurtbox>();
                if (hurtbox == null || !hurtbox.Active)
                    continue;

                Vector3 normal = hit.normal;
                bool canOverrideNormal = hitNormalIsVelocityDirection && _rigidbody && _rigidbody.velocity.sqrMagnitude > 0;
                if (canOverrideNormal) normal = -_rigidbody.velocity.normalized;

                Hit hitData = new Hit(
                    hit.point == Vector3.zero ? center : hit.point,
                    normal, Time.time, this, hurtbox
                );

                ValidateAndSendHit(hurtbox, hitData);
            }
        } else if (_collider is SphereCollider) {
            SphereCollider sphereCollider = _collider as SphereCollider;
            Vector3 center = sphereCollider.center;
            Collider[] colliders = Physics.OverlapSphere(
                transform.TransformPoint(sphereCollider.center),
                sphereCollider.radius * transform.lossyScale.x, _hurtboxMask, QueryTriggerInteraction.Collide);

            foreach (Collider collider in colliders) {
                IHurtbox hurtbox = collider.GetComponent<IHurtbox>();
                if (hurtbox == null || !hurtbox.Active)
                    continue;

                Vector3 normal = -(transform.position - collider.transform.position).normalized;

                bool canOverrideNormal = hitNormalIsVelocityDirection && _rigidbody && _rigidbody.velocity.sqrMagnitude > 0;
                if (canOverrideNormal) normal = -_rigidbody.velocity.normalized;

                Hit hitData = new Hit(
                    collider.transform.position - normal * sphereCollider.radius,
                    normal, Time.time, this, hurtbox
                );

                ValidateAndSendHit(hurtbox, hitData);
            }
        }
    }
}

}
