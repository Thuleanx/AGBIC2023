using Base;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using NaughtyAttributes;
using DG.Tweening;
using Utils;
using UI;

namespace GhostNirvana {

public class Reaper : MonoBehaviour {
    [SerializeField, Required] Miyu miyu;

    [SerializeField] Ease deathTimeSlowEase;
    [SerializeField] float deathTimeSlowDuration;
    [SerializeField] float deathTimeSlowDelaySeconds;
    [SerializeField] Canvas deathUI;

    [SerializeField] UnityEvent OnDeathStart;
    [SerializeField] Fader fader;

    bool reincarnating;

    void Start() {
        miyu.OnDeathEvent.AddListener(OnMiyuDeath);
    }

    void OnEnable() {
        deathUI.gameObject.SetActive(false);
    }

    void OnMiyuDeath() {
        StartCoroutine(IOnMiyuDeath());
    }

    IEnumerator IOnMiyuDeath() {
        OnDeathStart?.Invoke();

        yield return new WaitForSeconds(deathTimeSlowDelaySeconds);

        float t = 0;

        float lastTime = Time.unscaledTime;
        while (t < deathTimeSlowDuration) {
            yield return null;
            float deltaTime = Time.unscaledTime - lastTime;

            float easedT = 1 - EaseEvaluator.Evaluate(deathTimeSlowEase, t, deathTimeSlowDuration);

            Time.timeScale = easedT;

            t += deltaTime;
            lastTime = Time.unscaledTime;
        }

        Time.timeScale = 0;
        deathUI.gameObject.SetActive(true);
    }

    IEnumerator IReincarnation() {
        while (true) {
            var (successful, yieldInstruction) = fader.FadeIn();
            if (!successful) {
                yield return null;
                continue;
            }
            yield return yieldInstruction;
            break;
        }

        App.instance.RequestLoad(gameObject.scene.name);
        Time.timeScale = 1;
    }


    public void TriggerReincarnation() {
        if (reincarnating) return;
        reincarnating = true;
        // reload current level
        StartCoroutine(IReincarnation());
    }
}

}
