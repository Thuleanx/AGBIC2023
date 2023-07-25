using System.Collections.Generic;
using UnityEngine;
using CombatSystem;

namespace GhostNirvana.UI {

public class HealthBarManager : MonoBehaviour {
    [SerializeField] StatusTracker statusTrackerPrefab;
    [SerializeField] int poolExpansionRate = 20;
    [SerializeField] StatusRuntimeSet trackedStatusSet;

    Dictionary<Status, StatusTracker> statusTrackerMap = new Dictionary<Status, StatusTracker>();
    Queue<StatusTracker> statusTrackerPool = new Queue<StatusTracker>();

    protected void OnEnable() {
        trackedStatusSet.OnAdd += AddStatus;
        trackedStatusSet.OnRemove += RemoveStatus;
    }

    protected void OnDisable() {
        trackedStatusSet.OnAdd -= AddStatus;
        trackedStatusSet.OnRemove -= RemoveStatus;
    }

    public bool IsTrackingStatus(Status status) => statusTrackerMap.ContainsKey(status);

    void AddStatus(Status status) {
        if (IsTrackingStatus(status)) return; // avoid double adding
        if (statusTrackerPool.Count == 0) ExpandPool(poolExpansionRate);

        StatusTracker tracker = statusTrackerPool.Dequeue();

        tracker.TrackingStatus = status;
        tracker.gameObject.SetActive(true);
        statusTrackerMap[status] = tracker;
    }

    void RemoveStatus(Status status) {
        if (!IsTrackingStatus(status)) return; //nothing to remove

        StatusTracker tracker = statusTrackerMap[status];
        statusTrackerMap.Remove(status);
        if (tracker == null) return;
        tracker.TrackingStatus = null;
        if (tracker.gameObject != null)
            tracker.gameObject.SetActive(false);
        statusTrackerPool.Enqueue(tracker);
    }

    void ExpandPool(int count) {
        while (count-->0) {
            bool prefabIsActive = statusTrackerPrefab.gameObject.activeSelf;

            // Important: this prevents any OnEnables from running
            statusTrackerPrefab.gameObject.SetActive(false);
            statusTrackerPool.Enqueue(Instantiate(statusTrackerPrefab, transform));
            statusTrackerPrefab.gameObject.SetActive(prefabIsActive);
        }
    }
}

}
