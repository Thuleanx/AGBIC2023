using UnityEngine;

using Utils;

namespace GhostNirvana {

public class Arena : MonoBehaviour {
    public static Arena Instance;

    [SerializeField] Vector3 arenaHalfSize;
    [SerializeField] Vector3 arenaExtents;

    void Awake() {
        Instance = this;
    }

    public Vector3 GetRandomLocationInExtents() {
        float middleArea = arenaHalfSize.x * (2 * arenaExtents.z);
        float sideArea = 2 * arenaExtents.x * (arenaHalfSize.z + arenaExtents.z);

        bool sampleInsideMiddle = Mathx.RandomRange(0,1) *  (middleArea + sideArea) < middleArea;

        Vector3 point = Vector3.zero;
        if (sampleInsideMiddle) {
            point = new Vector3(
                Mathx.RandomRange(-arenaHalfSize.x, arenaHalfSize.x),
                0, Mathx.RandomRange(-arenaExtents.z, arenaExtents.z));
            point.z += Mathf.Sign(point.z) * arenaHalfSize.z;
        } else {
            point = new Vector3(
                Mathx.RandomRange(-arenaExtents.x, arenaExtents.x),
                0, Mathx.RandomRange(-(arenaHalfSize.z + arenaExtents.z), (arenaHalfSize.z + arenaExtents.z))
                );
            point.x += Mathf.Sign(point.x) * arenaHalfSize.x;
        }

        // convert to world space
        return transform.TransformPoint(point);
    }

    void OnDrawGizmos() {
        Gizmos.DrawWireCube(transform.position, 2 * arenaHalfSize);
        Gizmos.DrawWireCube(transform.position, 2 * (arenaHalfSize + arenaExtents));
    }
}

}
