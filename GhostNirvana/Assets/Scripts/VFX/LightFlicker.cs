using UnityEngine;
using System.Collections.Generic;

// Written by Steve Streeting 2017
// License: CC0 Public Domain http://creativecommons.org/publicdomain/zero/1.0/

/// <summary>
/// Component which will flicker a linked light while active by changing its
/// intensity between the min and max values given. The flickering can be
/// sharp or smoothed depending on the value of the smoothing parameter.
///
/// Just activate / deactivate this component as usual to pause / resume flicker
/// </summary>
namespace VFX {
    [RequireComponent(typeof(Renderer))]
    public class LightFlicker : MonoBehaviour {
        new Renderer renderer;
        Material material => renderer ? renderer.material : null;

        [SerializeField, ColorUsage(true, true)] Color dimColor;
        [SerializeField, ColorUsage(true, true)] Color brightColor;

        [Tooltip("Minimum random light intensity")]
        public float minIntensity = 0f;
        [Tooltip("Maximum random light intensity")]
        public float maxIntensity = 1f;
        [Tooltip("How much to smooth out the randomness; lower values = sparks, higher = lantern")]
        [Range(1, 50)]
        public int smoothing = 5;

        // Continuous average calculation via FIFO queue
        // Saves us iterating every time we update, we just change by the delta
        Queue<float> smoothQueue = new Queue<float>();
        float lastSum = 0;

        /// <summary>
        /// Reset the randomness and start again. You usually don't need to call
        /// this, deactivating/reactivating is usually fine but if you want a strict
        /// restart you can do.
        /// </summary>
        public void Reset() {
            smoothQueue.Clear();
            lastSum = 0;
        }

        protected void Awake() {
            renderer = GetComponent<Renderer>();
            // we create new instance so editing this won't edit other instances
            renderer.material = new Material(material);
        }

        void Start() {
            smoothQueue = new Queue<float>(smoothing);
        }

        void LateUpdate() {
            if (!material) return;

            // pop off an item if too big
            while (smoothQueue.Count >= smoothing) {
                lastSum -= smoothQueue.Dequeue();
            }

            // Generate random new item, calculate new average
            float newVal = Random.Range(minIntensity, maxIntensity);
            smoothQueue.Enqueue(newVal);
            lastSum += newVal;

            // Calculate new smoothed average
            float value = lastSum / (float)smoothQueue.Count;
            Color desiredEmission = Color.Lerp(dimColor, brightColor, value);
            material.SetColor("_EmissionColor", desiredEmission);
        }
    }
}
