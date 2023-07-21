using System;
using DG.Tweening;
using UnityEngine;
using DG.Tweening.Core.Easing;

namespace Utils {

public static class EaseEvaluator {
    const float _PiOver2 = Mathf.PI * 0.5f;
    const float _TwoPi = Mathf.PI * 2;

    public static float Evaluate(Ease ease, float time, float duration = 1, float overshootOrAmplitude = 1.70158f, float period = 0) {
        switch (ease) {
        case Ease.Linear:
            return time / duration;
        case Ease.InSine:
            return -(float)Math.Cos(time / duration * _PiOver2) + 1;
        case Ease.OutSine:
            return (float)Math.Sin(time / duration * _PiOver2);
        case Ease.InOutSine:
            return -0.5f * ((float)Math.Cos(Mathf.PI * time / duration) - 1);
        case Ease.InQuad:
            return (time /= duration) * time;
        case Ease.OutQuad:
            return -(time /= duration) * (time - 2);
        case Ease.InOutQuad:
            if ((time /= duration * 0.5f) < 1) return 0.5f * time * time;
            return -0.5f * ((--time) * (time - 2) - 1);
        case Ease.InCubic:
            return (time /= duration) * time * time;
        case Ease.OutCubic:
            return ((time = time / duration - 1) * time * time + 1);
        case Ease.InOutCubic:
            if ((time /= duration * 0.5f) < 1) return 0.5f * time * time * time;
            return 0.5f * ((time -= 2) * time * time + 2);
        case Ease.InQuart:
            return (time /= duration) * time * time * time;
        case Ease.OutQuart:
            return -((time = time / duration - 1) * time * time * time - 1);
        case Ease.InOutQuart:
            if ((time /= duration * 0.5f) < 1) return 0.5f * time * time * time * time;
            return -0.5f * ((time -= 2) * time * time * time - 2);
        case Ease.InQuint:
            return (time /= duration) * time * time * time * time;
        case Ease.OutQuint:
            return ((time = time / duration - 1) * time * time * time * time + 1);
        case Ease.InOutQuint:
            if ((time /= duration * 0.5f) < 1) return 0.5f * time * time * time * time * time;
            return 0.5f * ((time -= 2) * time * time * time * time + 2);
        case Ease.InExpo:
            return (time == 0) ? 0 : (float)Math.Pow(2, 10 * (time / duration - 1));
        case Ease.OutExpo:
            if (time == duration) return 1;
            return (-(float)Math.Pow(2, -10 * time / duration) + 1);
        case Ease.InOutExpo:
            if (time == 0) return 0;
            if (time == duration) return 1;
            if ((time /= duration * 0.5f) < 1) return 0.5f * (float)Math.Pow(2, 10 * (time - 1));
            return 0.5f * (-(float)Math.Pow(2, -10 * --time) + 2);
        case Ease.InCirc:
            return -((float)Math.Sqrt(1 - (time /= duration) * time) - 1);
        case Ease.OutCirc:
            return (float)Math.Sqrt(1 - (time = time / duration - 1) * time);
        case Ease.InOutCirc:
            if ((time /= duration * 0.5f) < 1) return -0.5f * ((float)Math.Sqrt(1 - time * time) - 1);
            return 0.5f * ((float)Math.Sqrt(1 - (time -= 2) * time) + 1);
        case Ease.InElastic:
            float s0;
            if (time == 0) return 0;
            if ((time /= duration) == 1) return 1;
            if (period == 0) period = duration * 0.3f;
            if (overshootOrAmplitude < 1) {
                overshootOrAmplitude = 1;
                s0 = period / 4;
            } else s0 = period / _TwoPi * (float)Math.Asin(1 / overshootOrAmplitude);
            return -(overshootOrAmplitude * (float)Math.Pow(2, 10 * (time -= 1)) * (float)Math.Sin((time * duration - s0) * _TwoPi / period));
        case Ease.OutElastic:
            float s1;
            if (time == 0) return 0;
            if ((time /= duration) == 1) return 1;
            if (period == 0) period = duration * 0.3f;
            if (overshootOrAmplitude < 1) {
                overshootOrAmplitude = 1;
                s1 = period / 4;
            } else s1 = period / _TwoPi * (float)Math.Asin(1 / overshootOrAmplitude);
            return (overshootOrAmplitude * (float)Math.Pow(2, -10 * time) * (float)Math.Sin((time * duration - s1) * _TwoPi / period) + 1);
        case Ease.InOutElastic:
            float s;
            if (time == 0) return 0;
            if ((time /= duration * 0.5f) == 2) return 1;
            if (period == 0) period = duration * (0.3f * 1.5f);
            if (overshootOrAmplitude < 1) {
                overshootOrAmplitude = 1;
                s = period / 4;
            } else s = period / _TwoPi * (float)Math.Asin(1 / overshootOrAmplitude);
            if (time < 1) return -0.5f * (overshootOrAmplitude * (float)Math.Pow(2, 10 * (time -= 1)) * (float)Math.Sin((time * duration - s) * _TwoPi / period));
            return overshootOrAmplitude * (float)Math.Pow(2, -10 * (time -= 1)) * (float)Math.Sin((time * duration - s) * _TwoPi / period) * 0.5f + 1;
        case Ease.InBack:
            return (time /= duration) * time * ((overshootOrAmplitude + 1) * time - overshootOrAmplitude);
        case Ease.OutBack:
            return ((time = time / duration - 1) * time * ((overshootOrAmplitude + 1) * time + overshootOrAmplitude) + 1);
        case Ease.InOutBack:
            if ((time /= duration * 0.5f) < 1) return 0.5f * (time * time * (((overshootOrAmplitude *= (1.525f)) + 1) * time - overshootOrAmplitude));
            return 0.5f * ((time -= 2) * time * (((overshootOrAmplitude *= (1.525f)) + 1) * time + overshootOrAmplitude) + 2);
        case Ease.InBounce:
            return Bounce.EaseIn(time, duration, overshootOrAmplitude, period);
        case Ease.OutBounce:
            return Bounce.EaseOut(time, duration, overshootOrAmplitude, period);
        case Ease.InOutBounce:
            return Bounce.EaseInOut(time, duration, overshootOrAmplitude, period);
        case Ease.INTERNAL_Zero:
            // 0 duration tween
            return 1;
        default:
            // OutQuad
            return -(time /= duration) * (time - 2);
        }
    }

}

}
