using UnityEngine;
using System;
using Base;

namespace Utils {
	public class Loading : MonoBehaviour {
		public SceneReference Next;
		void Start() {
		}
		void Update() {
			try {
                bool shouldProceedToScene = FMODUnity.RuntimeManager.HaveMasterBanksLoaded || Application.isEditor;
				if (shouldProceedToScene)
				{
					Debug.Log("Master Bank Loaded");
					App.instance.RequestLoad(Next.SceneName);
				} else {
					Debug.Log("Master Bank Not Yet Loaded " + FMODUnity.RuntimeManager.AnySampleDataLoading());
				}
			} catch (Exception err) {
				Debug.Log(err);
			}

		}
	}
}
