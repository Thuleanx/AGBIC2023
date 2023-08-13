using UnityEngine;
using System.Collections.Generic;
using NaughtyAttributes;
using VFX;
using ScriptableBehaviour;

namespace GhostNirvana.VFX {
    [RequireComponent(typeof(LightFlicker))]
    public class LightFlickerMoreByTime : MonoBehaviour {
        LightFlicker lightFlicker;

        Vector2 lightOnTime;
        Vector2 lightOffTime;

        [SerializeField] AnimationCurve onTimeScale;
        [SerializeField] AnimationCurve offTimeScale;
        [SerializeField] ScriptableFloat time;

        void Awake() {
            lightFlicker = GetComponent<LightFlicker>();

            lightOnTime = lightFlicker.lightOnTime;
            lightOffTime = lightFlicker.lightOffTime;
        }

        void Update() {
            lightFlicker.lightOnTime = lightOnTime * onTimeScale.Evaluate(time.Value);
            lightFlicker.lightOffTime = lightOffTime * offTimeScale.Evaluate(time.Value);
        }
    }
}
