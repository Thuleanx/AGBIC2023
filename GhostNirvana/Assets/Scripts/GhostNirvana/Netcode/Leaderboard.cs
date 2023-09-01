using UnityEngine;
using System.Collections.Generic;
using System;
using Dan.Main;

namespace GhostNirvana.Netcode {

[CreateAssetMenu(fileName = "Leaderboard",
    menuName = "~/Leaderboard", order = 1)]
public class Leaderboard : ScriptableObject {
    public string publicKey;

    const int Time_SIZE = 4;
    const int Money_SIZE = 6;
    const int ApplianceCount_SIZE = 4;

    public struct Record {
        public string Name;
        public int Rank;
        public int Time;
        public int MoneyAcquired;
        public int MoneyTakeHome;
        public int AppliancesRetrieved;
        public List<int> BuffsTaken;
    }

    public (bool, Record) Parse(Dan.Models.Entry entry) {
        string entryData = entry.Extra;
        Record record = new Record() {
            Name = entry.Username,
            Rank = entry.Rank,
        };
        int totalLen = Time_SIZE + 2*Money_SIZE + ApplianceCount_SIZE;
        if (entryData.Length < totalLen)
            return (false, record); // data is corrupted

        int currentParsingIndex = 0;
        int.TryParse(s: entryData.Substring(currentParsingIndex, Time_SIZE), 
            result: out record.Time);
        currentParsingIndex += Time_SIZE;
        int.TryParse(s: entryData.Substring(currentParsingIndex, Money_SIZE), 
            result: out record.MoneyAcquired);
        currentParsingIndex += Money_SIZE;
        int.TryParse(s: entryData.Substring(currentParsingIndex, Money_SIZE), 
            result: out record.MoneyTakeHome);
        currentParsingIndex += Money_SIZE;
        int.TryParse(s: entryData.Substring(currentParsingIndex, ApplianceCount_SIZE), 
            result: out record.AppliancesRetrieved);
        currentParsingIndex += ApplianceCount_SIZE;

        record.BuffsTaken = new List<int>();
        string buffsTaken = entryData.Substring(currentParsingIndex);
        for (int i = 0; i < buffsTaken.Length; i++) {
            char c = buffsTaken[i];
            int id = c;
            record.BuffsTaken.Add(id);
        }
        Debug.Log(entryData + " " + buffsTaken.Length);

        return (true, record);
    }

    public void Save(Record record, Action<bool> regularCallback, Action<string> errorCallback) {
        string entryData = "";

        entryData += string.Format("{0:0000}", record.Time);
        entryData += string.Format("{0:000000}", record.MoneyAcquired);
        entryData += string.Format("{0:000000}", record.MoneyTakeHome);
        entryData += string.Format("{0:0000}", record.AppliancesRetrieved);
        foreach (int buffID in record.BuffsTaken) {
            Debug.Log(buffID);
            entryData += (char) buffID;
        }
        Debug.Log(entryData + " " + record.BuffsTaken.Count);

        LeaderboardCreator.ResetPlayer(() => {
            LeaderboardCreator.UploadNewEntry(
                publicKey,
                username: record.Name,
                score: record.MoneyTakeHome,
                extra: entryData,
                regularCallback,
                errorCallback
            );
        });
    }

    public void Load(int numberOfEntries, Action<List<Record>> callback) {
        LeaderboardCreator.GetLeaderboard(
            publicKey,
            isInAscendingOrder: false,
            searchQuery: new Dan.Models.LeaderboardSearchQuery() {
                Skip = 0,
                Take = numberOfEntries
            },
            callback: (entries) => {
                List<Record> records = new List<Record>();
                for (int i = 0; i < entries.Length; i++) {
                    var (successful, value) = Parse(entries[i]);
                    records.Add(value);
                }
                callback?.Invoke(records);
            }
        );
    }
}

}
