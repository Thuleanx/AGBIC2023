using UnityEngine;
using NaughtyAttributes;
using Optimization;
using Base;

namespace ComposableBehaviour {

public class TriggerableSpawner : MonoBehaviour {
    [SerializeField, ShowAssetPreview] GameObject spawnObject;
    [SerializeField] bool inheritRotation;

    [Button]
    public void Spawn() => Spawn(transform.position);
    public void Spawn(Vector3 pos) {
        if (!spawnObject) return;
        ObjectPoolManager.Instance?.Borrow(
            App.GetActiveScene(),
            spawnObject.transform,
            transform.position,
            inheritRotation ? transform.rotation : Quaternion.identity
        );
    }
}

}
