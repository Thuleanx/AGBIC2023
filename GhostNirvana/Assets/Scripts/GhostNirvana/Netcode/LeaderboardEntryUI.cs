using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GhostNirvana.Netcode {
    public class LeaderboardEntryUI : MonoBehaviour {
        [SerializeField] TMP_Text Name;
        [SerializeField] TMP_Text Rank;
        [SerializeField] TMP_Text MoneyTakeHome;
        [SerializeField] TMP_Text Time;
        [SerializeField] Image mostBoughtItem;
        [SerializeField] BuffList buffs;

        public void Set(Leaderboard.Record record) {
            Name.text = record.Name;
            Rank.text = record.Rank.ToString();
            MoneyTakeHome.text = String.Format("{0:C}", record.MoneyTakeHome/100.0f);

            int secondsInMinute = 60;
            int timeMinutes = record.Time / secondsInMinute;
            int timeSeconds= record.Time % secondsInMinute;

            Time.text = String.Format("{0:00}:{1:00}", timeMinutes, timeSeconds);

            Dictionary<int, int> acquisitionFrequency = new Dictionary<int, int>();
            int mostBoughtItemID = -1;
            foreach (int buffID in record.BuffsTaken) {
                if (!acquisitionFrequency.ContainsKey(buffID))
                    acquisitionFrequency[buffID] = 0;
                acquisitionFrequency[buffID]++;

                if (mostBoughtItemID == -1) {
                    mostBoughtItemID = buffID;
                    continue;
                }

                int fcurrent = acquisitionFrequency[buffID];
                int fbest = acquisitionFrequency[mostBoughtItemID];

                bool cmp1 = fcurrent >= fbest;
                bool cmp2 = buffID >= mostBoughtItemID && fcurrent == fbest;

                if (cmp1 || cmp2)
                    mostBoughtItemID = buffID;
            }

            mostBoughtItem.enabled = false;

            foreach (Buff buff in buffs.All) {
                if (buff.id == mostBoughtItemID) {
                    mostBoughtItem.enabled = true;
                    mostBoughtItem.sprite = buff.icon;
                    break;
                }
            }
        }
    }
}
