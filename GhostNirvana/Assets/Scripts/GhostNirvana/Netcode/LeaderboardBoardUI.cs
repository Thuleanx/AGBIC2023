using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;

namespace GhostNirvana.Netcode {

public class LeaderboardBoardUI : MonoBehaviour {
    [SerializeField] Leaderboard leaderboard;
    [SerializeField] LeaderboardEntryUI entry;
    [SerializeField] int maxEntries = 100;
    [SerializeField] Transform leaderboardTable;
    [SerializeField, ReadOnly] List<LeaderboardEntryUI> liveEntries;

    [SerializeField] float closeDelay;
    [SerializeField] UnityEvent onClose;
    [SerializeField] UnityEvent onCloseEnd;

    public void LoadAllLeaderboardEntries() {
        leaderboard.Load(maxEntries, OnEntriesLoaded);
    }

    void OnEntriesLoaded(List<Leaderboard.Record> records) {
        foreach (Leaderboard.Record record in records) {
            LeaderboardEntryUI entryUI = Instantiate(entry, parent: leaderboardTable);
            entryUI.Set(record);
            liveEntries.Add(entryUI);
        }
    }

    void OnEnable() {
        LoadAllLeaderboardEntries();
    }

    void OnDisable() {
        foreach (LeaderboardEntryUI entry in liveEntries)
            Destroy(entry.gameObject);
        liveEntries.Clear();
    }

    public void Close() => StartCoroutine(IClose());
    IEnumerator IClose() {
        onClose?.Invoke();
        yield return new WaitForSecondsRealtime(closeDelay);
        onCloseEnd?.Invoke();
        gameObject.SetActive(false);
    }
}

}
