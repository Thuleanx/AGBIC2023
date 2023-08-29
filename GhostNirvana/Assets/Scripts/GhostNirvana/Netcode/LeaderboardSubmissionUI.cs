using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GhostNirvana.Netcode {
    public class LeaderboardSubmissionUI : MonoBehaviour {
        [SerializeField] Leaderboard leaderboard;
        [SerializeField] AchievementTracker tracker;

        [SerializeField] TMP_InputField nameInput;
        [SerializeField] Button submissionButton;

        public void OnSubmit() {
            nameInput.interactable = false;
            submissionButton.interactable = false;
            leaderboard.Save(
                record: new Leaderboard.Record() {
                    Name = nameInput.text,
                    Time = tracker.Time,
                    MoneyAcquired = tracker.MoneyEarned,
                    MoneyTakeHome = tracker.MoneyTakeHome,
                    AppliancesRetrieved = tracker.ApplianceCollected,
                    BuffsTaken = tracker.BuffsTaken
                },
                OnSubmissionFinished,
                OnSubmissionError
            );
        }

        void OnSubmissionFinished(bool successful) {
            Debug.Log("successful: " + successful);
        }

        void OnSubmissionError(string error) {
            Debug.LogError(error);

            nameInput.interactable = true;
            submissionButton.interactable = true;
        }
    }
}
