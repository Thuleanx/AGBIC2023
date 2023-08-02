using System;
using UnityEngine;
using TMPro;
using ScriptableBehaviour;

namespace GhostNirvana.UI {
    [RequireComponent(typeof(TMP_Text))]
    public class TimeDisplay : MonoBehaviour {
        TMP_Text textMeshProObj;
        [SerializeField] ScriptableFloat timeElapsed;
        
        void Awake() {
            textMeshProObj = GetComponent<TMP_Text>();
        }

        void LateUpdate() {
            int secondsInMinutes = 60;
            int numMinutes = (int) (timeElapsed.Value);
            int numSeconds = (int) (timeElapsed.Value * secondsInMinutes) % secondsInMinutes;
            Debug.Log(timeElapsed.Value);
            textMeshProObj.text = String.Format("{0,2:D2}:{1,2:D2}", numMinutes, numSeconds);
        }
    }
}
