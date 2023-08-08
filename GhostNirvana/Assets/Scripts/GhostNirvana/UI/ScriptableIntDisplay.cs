using System;
using UnityEngine;
using TMPro;
using ScriptableBehaviour;
using NaughtyAttributes;

namespace GhostNirvana.UI {
    [RequireComponent(typeof(TMP_Text))]
    public class ScriptableIntDisplay : MonoBehaviour {
        TMP_Text textContainer;
        [SerializeField] Scriptable<int> scriptableInt;
        [SerializeField] float scale;
        [SerializeField, ResizableTextArea] string format;
        
        void Awake() {
            textContainer = GetComponent<TMP_Text>();
        }

        void LateUpdate() {
            textContainer.text = String.Format(format, scriptableInt.Value * scale);
        }
    }
}
