using System;
using UnityEngine;
using TMPro;
using ScriptableBehaviour;

namespace GhostNirvana.UI {
    [RequireComponent(typeof(TMP_Text))]
    public class IntegerTextDisplay : MonoBehaviour {
        TMP_Text textMeshProObj;
        [SerializeField] Scriptable<int> trackedInteger;
        
        void Awake() {
            textMeshProObj = GetComponent<TMP_Text>();
        }

        void LateUpdate() {
            textMeshProObj.text = String.Format("{0:C2}", trackedInteger.Value / 100.0f);
        }
    }
}
