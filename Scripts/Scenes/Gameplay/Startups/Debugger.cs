using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Startups {
	public class Debugger : MonoBehaviour {
		AudioController audioController;

		void Start() {
			audioController = Object.FindObjectOfType(typeof(AudioController)) as AudioController;

			audioController.Load("rockstar", "Audio/Music/EngineTest");
			audioController.Load("forest_ambience", "Audio/Music/Forest_Ambience");
			audioController.Load("forest_background", "Audio/Music/Forest_Background");

			audioController.Play("rockstar", AudioController.Mode.JUMP, 5f, 15);

//			StartCoroutine(DebugLoopMusic(10f));
		}

		IEnumerator DebugLoopMusic(float duration) {
			for(;;) {
				audioController.PauseFadeOutAll(3f, new List<string> {"forest_background"});
				audioController.UnpauseFadeIn("forest_background", 3f, AudioController.Mode.LOOP);
				yield return new WaitForSeconds(duration);
				audioController.PauseFadeOutAll(3f, new List<string> {"forest_ambience"});
				audioController.UnpauseFadeIn("forest_ambience", 3f, AudioController.Mode.LOOP);
				yield return new WaitForSeconds(duration);
			}
		}
	}
}