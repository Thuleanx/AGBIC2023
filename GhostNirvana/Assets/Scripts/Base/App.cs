using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using PrettyPatterns;
using DG.Tweening;

namespace Base {
	public class App : Singleton<App> {
		public override void Awake() {
			base.Awake();

			SceneManager.sceneLoaded += (scene, loadmode) => {
				AfterSceneLoad?.Invoke(scene, loadmode);
			};
			AfterSceneLoad = new UnityEvent<Scene, LoadSceneMode>();
			BeforeSceneUnload = new UnityEvent<Scene>();
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		public static void Bootstrap() {
			GameObject app = FindObjectOfType<App>()?.gameObject;
			if (app == null) {
				app = UnityEngine.Object.Instantiate(Resources.Load("App")) as GameObject;
				if (app == null) throw new ApplicationException();
			}
			UnityEngine.Object.DontDestroyOnLoad(app);
            DOTween.SetTweensCapacity(50000, 125);
		}

		#region App Scene Management
		public static UnityEvent<Scene, LoadSceneMode> AfterSceneLoad;
		public static UnityEvent<Scene> BeforeSceneUnload;
		#endregion

		public static Scene GetActiveScene() => SceneManager.GetActiveScene();

		public void RequestLoad(string sceneName, LoadSceneMode mode = LoadSceneMode.Single) {
			if (mode == LoadSceneMode.Single) TriggerBeforeUnload();
			SceneManager.LoadScene(sceneName, mode);
		}
		public void RequestLoadAsync(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
			=> StartCoroutine(DirectLoadAsync(sceneName, mode));
		public IEnumerator DirectLoadAsync(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)  {
			if (mode == LoadSceneMode.Single) TriggerBeforeUnload();
			AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
			asyncLoad.allowSceneActivation = false;
			while (!asyncLoad.isDone) {
				if (asyncLoad.progress >= 0.9f)
					asyncLoad.allowSceneActivation = true;
				yield return null;
			}
		}
		public void RequestUnload(string sceneName, UnloadSceneOptions options = UnloadSceneOptions.None)
			=> StartCoroutine(_UnloadNextFrame(sceneName, options));
		public IEnumerator RequestUnloadAsync(string sceneName, UnloadSceneOptions options = UnloadSceneOptions.None) {
			yield return _UnloadNextFrame(sceneName, options);
		}

		HashSet<string> Unloading = new HashSet<string>();
		IEnumerator _UnloadNextFrame(string sceneName, UnloadSceneOptions options) {
			if (SceneManager.GetSceneByName(sceneName).isLoaded && !Unloading.Contains(sceneName)) {
				Unloading.Add(sceneName);
				BeforeSceneUnload?.Invoke(SceneManager.GetSceneByName(sceneName));
				yield return null;
				yield return SceneManager.UnloadSceneAsync(sceneName, options);
				Unloading.Remove(sceneName);
			} else {
				Debug.Log("Unsuccessful unload: Scene " + sceneName + " is either not loaded or is currently unloading");
				yield return null;
			}
		}

		public void TriggerBeforeUnload() {
			for (int i = 0; i < SceneManager.sceneCount; i++) {
				Scene sceneAt = SceneManager.GetSceneAt(i);
				if (sceneAt.isLoaded)
					BeforeSceneUnload?.Invoke(sceneAt);
			}
		}

		public void RequestReload() {
			TriggerBeforeUnload();
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}

	}
}
