using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CombatSystem;

namespace GhostNirvana {

public class HealthBarManager : MonoBehaviour {
    public static HealthBarManager Instance;

    [SerializeField] StatusTracker statusTrackerPrefab;
    [SerializeField] int poolExpansionRate = 20;

    Dictionary<Status, StatusTracker> statusTrackerMap = new Dictionary<Status, StatusTracker>();
    Queue<StatusTracker> statusTrackerPool = new Queue<StatusTracker>();

    void Awake() => Instance = this;

    public void AddStatus(Status status) {
        if (statusTrackerPool.Count == 0) ExpandPool(poolExpansionRate);

        StatusTracker tracker = statusTrackerPool.Dequeue();

        tracker.TrackingStatus = status;
        tracker.gameObject.SetActive(true);
        statusTrackerMap[status] = tracker;
    }

    public void RemoveStatus(Status status) {
        if (!statusTrackerMap[status]) return;
        statusTrackerPool.Enqueue(statusTrackerMap[status]);

        statusTrackerMap[status].TrackingStatus = null;
        if (statusTrackerMap[status].gameObject)
            statusTrackerMap[status].gameObject.SetActive(false);

        statusTrackerMap.Remove(status);
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
