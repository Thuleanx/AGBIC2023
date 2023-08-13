using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

namespace Audio {
	public class EventInstanceEmitter : MonoBehaviour {
		[SerializeField] EventReference eventReference;

		public void PlayOneShot() => FMODUnity.RuntimeManager.PlayOneShot(eventReference);
	}
}
