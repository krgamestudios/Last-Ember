using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour {
	//public structures
	public enum Mode {
		NONE,
		ONCE,
		LOOP,
		JUMP
	}

	public struct AudioContainer {
		public AudioSource source;
		public Mode mode;
		public float jumpStart;
		public float jumpEnd;
	}

	//internals
	Dictionary<string, AudioContainer> audioDictionary = new Dictionary<string, AudioContainer>();
	static bool initialized = false;

	//monobehaviour methods
	void Start() {
		if (initialized) {
			Destroy(gameObject);
		}

		initialized = true;

		DontDestroyOnLoad(gameObject);
	}

	void Update() {
		foreach(KeyValuePair<string, AudioContainer> iter in audioDictionary) {
			//handle the jump points
			if (iter.Value.mode == Mode.JUMP && iter.Value.jumpStart >= 0f && iter.Value.jumpEnd > 0f) {
				if (iter.Value.source.time >= iter.Value.jumpEnd) {
					iter.Value.source.time = iter.Value.jumpStart;
				}
			}
		}
	}

	void OnDestroy() {
		foreach(KeyValuePair<string, AudioContainer> iter in audioDictionary) {
			Resources.UnloadAsset(iter.Value.source.clip);
			Destroy(iter.Value.source);
		}
	}

	//public access members
	public void Load(string name, string filename) {
		AudioContainer container = new AudioContainer();

		container.source = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
		container.source.clip = Resources.Load<AudioClip>(filename) as AudioClip;
		container.source.volume = 0f;
		container.mode = Mode.NONE;

		audioDictionary[name] = container;
	}

	public bool Unload(string name) {
		if (!audioDictionary.ContainsKey(name)) {
			return false;
		}

		AudioContainer container = audioDictionary[name];

		Resources.UnloadAsset(container.source.clip);
		Destroy(container.source);

		audioDictionary.Remove(name);
		return true;
	}

	//controls
	public void Play(string name, Mode mode = Mode.ONCE, float jumpStart = -1f, float jumpEnd = -1f) {
		AudioContainer container = audioDictionary[name];

		container.source.Play();
		container.source.loop = mode == Mode.LOOP;
		container.source.volume = 1f;
		container.mode = mode;
		container.jumpStart = jumpStart;
		container.jumpEnd = jumpEnd;
		audioDictionary[name] = container;
	}

	public void Pause(string name) {
		AudioContainer container = audioDictionary[name];

		container.source.Pause();
	}

	public void Unpause(string name, Mode mode = Mode.ONCE, float jumpStart = -1f, float jumpEnd = -1f) {
		AudioContainer container = audioDictionary[name];

		if (container.source.isPlaying) {
			container.source.UnPause();
		} else {
			Play(name, mode, jumpStart, jumpEnd);
		}
	}

	public void Stop(string name) {
		AudioContainer container = audioDictionary[name];

		container.source.Stop();
		container.mode = Mode.NONE;

		audioDictionary[name] = container;
	}

	public void StopAll() {
		List<string> names = new List<string>();
		foreach(KeyValuePair<string, AudioContainer> iter in audioDictionary) {
			names.Add(iter.Key);
		}

		foreach(string name in names) {
			Stop(name);
		}
	}

	//fade controls
	public void FadeIn(string name, float seconds) {
		StartCoroutine(FadeInCallback(audioDictionary[name].source, 1f/seconds));
	}

	IEnumerator FadeInCallback(AudioSource source, float amountPerSecond) {
		source.volume = 0;
		while (source.volume < 1f) {
			yield return new WaitForSeconds(0.1f);
			source.volume += amountPerSecond / 10f;
		}
	}

	public void FadeOut(string name, float seconds) {
		StartCoroutine(FadeOutCallback(audioDictionary[name].source, 1f/seconds));
	}

	IEnumerator FadeOutCallback(AudioSource source, float amountPerSecond) {
		while (source.volume > 0f) {
			yield return new WaitForSeconds(0.1f);
			source.volume -= amountPerSecond / 10f;
		}
	}

	//hybrid controls
	public void PlayFadeIn(string name, float seconds, Mode mode = Mode.ONCE, float jumpStart = -1f, float jumpEnd = -1f) {
		FadeIn(name, seconds);
		Play(name, mode, jumpStart, jumpEnd);
	}

	public void PauseFadeOut(string name, float seconds) {
		FadeOut(name, seconds);
		StartCoroutine(PauseFadeOutCallback(name, seconds));
	}

	public void PauseFadeOutAll(float seconds, List<string> exclude = null) {
		foreach(KeyValuePair<string, AudioContainer> iter in audioDictionary) {
			if (exclude != null && exclude.Contains(iter.Key)) {
				continue;
			}
			FadeOut(iter.Key, seconds);
			StartCoroutine(PauseFadeOutCallback(iter.Key, seconds));
		}
	}

	IEnumerator PauseFadeOutCallback(string name, float seconds) {
		yield return new WaitForSeconds(seconds);
		Pause(name);
	}

	public void UnpauseFadeIn(string name, float seconds, Mode mode = Mode.ONCE, float jumpStart = -1f, float jumpEnd = -1f) {
		Unpause(name, mode, jumpStart, jumpEnd);
		FadeIn(name, seconds);
	}

	public void StopFadeOut(string name, float seconds) {
		FadeOut(name, seconds);
		StartCoroutine(StopFadeOutCallback(name, seconds));
	}

	public void StopFadeOutAll(float seconds, List<string> exclude = null) {
		foreach(KeyValuePair<string, AudioContainer> iter in audioDictionary) {
			if (exclude != null && exclude.Contains(iter.Key)) {
				continue;
			}
			FadeOut(iter.Key, seconds);
			StartCoroutine(StopFadeOutCallback(iter.Key, seconds));
		}
	}

	IEnumerator StopFadeOutCallback(string name, float seconds) {
		yield return new WaitForSeconds(seconds);
		Stop(name);
	}

	//status
	public bool GetPlaying(string name) {
		return audioDictionary[name].source.isPlaying;
	}

	public Mode GetMode(string name) {
		return audioDictionary[name].mode;
	}
}
