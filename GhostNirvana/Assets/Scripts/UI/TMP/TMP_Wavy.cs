using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using NaughtyAttributes;
using Utils;

namespace UI {

public class TMP_Wavy : TMPManipulator
{
    public enum WaveCutoff {
        PER_CHARACTER,
        PER_WORD
    }

    [SerializeField, Range(0, 1)] float waveSpread;
	[SerializeField] Vector2 woobleScale;
    [SerializeField] Vector2 woobleFrequency;
    [SerializeField, MinMaxSlider(0, 5f)] Vector2 woobleTimeOffset;
    [SerializeField] WaveCutoff cutoff;

    float timeOffset;

    void OnEnable() {
        timeOffset = Mathx.RandomRange(woobleTimeOffset) * 2 * Mathf.PI;
    }

	// Update is called once per frame
	public override void Update()
	{
		base.Update();
		int p = 0;
		Vector3 offset = Wobble(Time.unscaledTime + (p++) * waveSpread);

		for (int i = 0; i < textMesh.textInfo.characterCount; i++) {
			TMP_CharacterInfo c = textMesh.textInfo.characterInfo[i];
			if (c.character == ' ') {
				offset = Wobble(Time.unscaledTime + (p++) * waveSpread);
			} else {
				int index = c.vertexIndex;
				for (int j = 0; j < 4; j++)
					vertices[index + j] = vertices[index + j] + offset;

                if (cutoff == WaveCutoff.PER_CHARACTER)
				    offset = Wobble(Time.unscaledTime + (p++) * waveSpread);
			}
		}
	}

	Vector2 Wobble(float time) => new Vector2(
        Mathf.Sin(time * woobleFrequency.x * 2 * Mathf.PI + timeOffset) * woobleScale.x,
        Mathf.Cos(time * woobleFrequency.y * 2 * Mathf.PI + timeOffset) * woobleScale.y
    );
}
}
