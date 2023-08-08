using System;
using UnityEngine;
using System.Collections;
using NaughtyAttributes;
using DG.Tweening;
using Utils;

namespace GhostNirvana {

public class Reaper : MonoBehaviour {
    [SerializeField, Required] Miyu miyu;

    [SerializeField] Ease deathTimeSlowEase;
    [SerializeField] float deathTimeSlowDuration;
    [SerializeField] float deathTimeSlowDelaySeconds;

    void Start() {
        miyu.OnDeathEvent.AddListener(OnMiyuDeath);
    }

    void OnMiyuDeath() {
        Debug.Log("HI");
        StartCoroutine(IOnMiyuDeath());
    }

    IEnumerator IOnMiyuDeath() {
        yield return new WaitForSeconds(deathTimeSlowDelaySeconds);

        float t = 0;

        float lastTime = Time.unscaledTime;
        while (t < deathTimeSlowDuration) {
            yield return null;
            float deltaTime = Time.unscaledTime - lastTime;

            float easedT = 1 - EaseEvaluator.Evaluate(deathTimeSlowEase, t, deathTimeSlowDuration);

            Time.timeScale = easedT;

            lastTime = deltaTime;
        }
        Time.timeScale = 0;
    }
}

}
