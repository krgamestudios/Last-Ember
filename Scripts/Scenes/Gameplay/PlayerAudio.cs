using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpriterDotNetUnity;

//DOCS: this is intended for Ember's foot step sounds

public class PlayerAudio : MonoBehaviour {
	//public members
	public AudioClip[] leftFootsteps;
	public AudioClip[] rightFootsteps;
	public AudioClip jumpSound;
	public AudioClip landSound;

	//internal members
	GameObject spriteObject;
	UnityAnimator animator;
	AudioSource audioSource;

	void Start() {
		audioSource = GetComponent<AudioSource>();
	}

	void Update() {
		//spriter object is handled as an animation
		HandleAnimation();
	}

	// Update is called once per frame
	void HandleAnimation() {
		if (spriteObject == null) {
			foreach (Transform child in transform) {
				if (child.name ==  "Ember") {
					spriteObject = child.gameObject;
					break;
				}
			}
		}

		if (animator == null && spriteObject != null) {
			animator = spriteObject.GetComponent<SpriterDotNetBehaviour>().Animator;
			animator.EventTriggered += AudioTriggers;
		}

	}

	void AudioTriggers(string name) {
		switch(name) {
			case "S: footstep left":
				audioSource.PlayOneShot(leftFootsteps[PickASlot(leftFootsteps.Length)]);
				break;

			case "S: footstep right":
				audioSource.PlayOneShot(rightFootsteps[PickASlot(rightFootsteps.Length)]);
				break;

			case "S: jump":
				audioSource.PlayOneShot(jumpSound);
				break;

			case "S: land":
				audioSource.PlayOneShot(landSound);
				break;
		}
	}

	int PickASlot(int length) {
		for (int i = 0; i < length; i++) {
			if (Random.Range(0, 2) == 0) return i;
		}
		return 0;
	}
}
