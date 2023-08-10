using UnityEngine;
using NaughtyAttributes;
using ScriptableBehaviour;
using Optimization;

namespace ComposableBehaviour {

public class SpawnOnInterval : MonoBehaviour {
    [SerializeField] ScriptableFloat spawnSpeed;
    [SerializeField, ShowAssetPreview, Required] GameObject prefab;

    float spawnProgress = 0;

    void Update() {
        if (!spawnSpeed || !prefab || spawnSpeed.Value == 0) return;
        spawnProgress += Time.deltaTime * spawnSpeed.Value;
        while (spawnProgress >= 1) {
            ObjectPoolManager.Instance.Borrow(
                gameObject.scene, prefab.transform,
                transform.position, Quaternion.identity
            );
            spawnProgress--;
        }
    }
}

}
