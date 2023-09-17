using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

namespace GhostNirvana.Netcode {
    public class LeaderboardSubmissionUI : MonoBehaviour {
        [SerializeField] Leaderboard leaderboard;
        [SerializeField] AchievementTracker tracker;

        [SerializeField] TMP_InputField nameInput;
        [SerializeField] TMP_Text errorText;
        [SerializeField] Button submissionButton;

        [SerializeField] UnityEvent OnSubmitEvent;
        [SerializeField] UnityEvent OnSubmitSuccessEvent;
        [SerializeField] UnityEvent<string> OnSubmitFailureEvent;

        void OnEnable() {
            errorText.text = "";
        }

        public void OnSubmit() {
            OnSubmitEvent?.Invoke();
            nameInput.interactable = false;
            submissionButton.interactable = false;
            errorText.text = "Submitting...";
            if (nameInput.text.Length == 0) {
                OnSubmissionError("Name cannot be empty.");
                return;
            }
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
            if (successful) {
                OnSubmitSuccessEvent?.Invoke();
                errorText.text = "Submitted";
            }
        }

        void OnSubmissionError(string error) {
            if (error.Length == 0) return;
            Debug.LogError(error);
            nameInput.interactable = true;
            submissionButton.interactable = true;

            OnSubmitFailureEvent?.Invoke(error);
            errorText.text = error;
        }
    }
}
