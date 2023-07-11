using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Thuleanx.Utils {
	public struct Timer {
		public float Duration { get; private set; }
		public bool Paused { get; private set; }
		float timeLeftLagged;

		public float TimeLeft {
			get {
				if (!Paused)
					timeLeftLagged = Mathf.Max(timeLeftLagged - (Time.time - TimeLastSampled), 0);
				TimeLastSampled = Time.time;
				return timeLeftLagged;
			}
			set {
				timeLeftLagged = value;
				TimeLastSampled = Time.time;
			}
		}
		public float TimeLastSampled { get; private set; }
		public float ElapsedFraction { get => 1 - TimeLeft / Duration; }

		public Timer(float durationSeconds, bool pausedDefault = false) {
			Duration = durationSeconds;
			timeLeftLagged = 0;
			TimeLastSampled = Time.time;
			Paused = pausedDefault;
			if (!pausedDefault) Start();
		}

		public void Start() {
			TimeLeft = Duration;
			Paused = false;
		}
		public void Pause() {
			float left = TimeLeft;
			Paused = true;
		}
		public void UnPause() {
			float left = TimeLeft;
			Paused = false;
		}
		public void Stop() { TimeLeft = 0; }

		public static implicit operator bool(Timer timer) => timer.TimeLeft > 0;
		public static implicit operator Timer(float durationSeconds) => new Timer(durationSeconds);
	}
}