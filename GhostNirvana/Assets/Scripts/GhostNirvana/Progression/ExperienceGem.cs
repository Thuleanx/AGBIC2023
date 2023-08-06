using UnityEngine;
using System.Collections;
using DG.Tweening;
using Optimization;
using ScriptableBehaviour;

using Utils;

namespace GhostNirvana {

[RequireComponent(typeof(Collider))]
public class ExperienceGem : PoolableEntity {
    new Collider collider;

    [SerializeField] float xpYieldPerPickup;
    [SerializeField] float collectionDuration;
    [SerializeField] Ease absorptionEase;
    [SerializeField] LinearLimiterFloat miyuXP;
    [SerializeField] float collectionForceRange = .5f;
    Tween collectionTween;

    void Awake() {
        collider = GetComponent<Collider>();
    }

	protected override void OnEnable() {
		base.OnEnable();
		collider.enabled = true;
	}

    void OnTriggerEnter(Collider other) {
        Miyu miyu = other.GetComponentInParent<Miyu>();
        bool isPlayer = miyu;
        if (!isPlayer) return;
        collider.enabled = false;
        StartCoroutine(CollectGem(miyu.transform));
    }

    protected override IEnumerator IDispose() {
        yield return base.IDispose();
    }

    public IEnumerator CollectGem(Transform playerTransform) {
        float t = 0;
        float lastTime = Time.time;
        Vector3 originalLocation = transform.position;
        while (t < collectionDuration) {
            yield return null;
            float deltaTime = Time.time - lastTime;

            float easedT = EaseEvaluator.Evaluate(absorptionEase, t, collectionDuration);
            transform.position = Vector3.Lerp(originalLocation, playerTransform.position, easedT);

            Vector3 playerDisplacement = playerTransform.position - transform.position;
            playerDisplacement.y = 0;

            float sqrDistanceToPlayer = Vector3.Dot(playerDisplacement, playerDisplacement);
            if (sqrDistanceToPlayer < collectionForceRange * collectionForceRange) {
                break;
            }

            t += deltaTime;
            lastTime = Time.time;
        }

        miyuXP.Value += xpYieldPerPickup;
        this.Dispose();
    }
}

}
