using UnityEngine;
using System.Collections.Generic;
using NaughtyAttributes;
using Utils;

namespace VFX {
    [RequireComponent(typeof(Renderer))]
    public class LightFlicker : MonoBehaviour {
        new Renderer renderer;
        Material material => renderer ? renderer.material : null;

        [SerializeField, ColorUsage(true, true)] Color dimColor;
        [SerializeField, ColorUsage(true, true)] Color brightColor;

        [SerializeField, MinMaxSlider(0.0f,1.0f)] Vector2 lightOnTime;
        [SerializeField, MinMaxSlider(0.0f,1.0f)] Vector2 lightOffTime;

        bool isOn;
        float timeUntilSwitch;

        void Awake() {
            renderer = GetComponent<Renderer>();
            renderer.material = new Material(renderer.material);
        }

        void OnEnable() {
            isOn = Mathx.RandomRange(0.0f,1.0f) < 0.5f;
        }

        void LateUpdate() {
            timeUntilSwitch -= Time.deltaTime;

            if (timeUntilSwitch <= 0) {
                isOn ^= true;
                timeUntilSwitch = Mathx.RandomRange(isOn ? lightOnTime : lightOffTime);
            }

            if (isOn)   material.EnableKeyword("_EMISSION");
            else        material.DisableKeyword("_EMISSION");
        }
    }
}
