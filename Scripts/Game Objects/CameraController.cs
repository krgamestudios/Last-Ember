using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
	//public access members
	public GameObject targetObject;
	public Vector3 offset;
	public float lerpSpeed = 2f;

	Vector2 peek = new Vector2(0, 0);

	//private members
	Vector3 virtualLocation;

	void Start() {
		virtualLocation = transform.position;
	}

	void Update() {
		//only continue if the target has been set
		if (targetObject == null) {
			return;
		}

		//cache the position we want to move to
		Vector3 targetPosition = targetObject.transform.position + offset + new Vector3(peek.x, peek.y, 0f);

		//If the distance is small, short circuit the lerp, so we don't have sudden pops in camera motion.
		if ((targetPosition - virtualLocation).sqrMagnitude > 0.01f) {
			//Interpolate to the target location.
			virtualLocation = Vector3.Lerp(virtualLocation, targetPosition, lerpSpeed * Time.deltaTime);

			//Snap to pixel coordinates
			Vector3 snapped = virtualLocation;

			snapped.x = Mathf.Round(snapped.x * 100) / 100;
			snapped.y = Mathf.Round(snapped.y * 100) / 100;

			snapped.z = transform.position.z; //BUGFIX

			transform.position = snapped;
		}
	}

	public Vector2 GetPeek() {
		return peek;
	}

	public void SetPeek(Vector2 newPeek, float delay = 0.5f) {
		peek = new Vector2(0f, 0f);
		StartCoroutine(SetPeekAfter(delay, newPeek)); //NOTE: a delay to peeking, for smooth gameplay
	}

	IEnumerator SetPeekAfter(float delay, Vector2 addition) {
		yield return new WaitForSeconds(delay);
		peek = addition;
	}

	public void ResetPeek() {
		peek = new Vector2(0, 0);
	}
}