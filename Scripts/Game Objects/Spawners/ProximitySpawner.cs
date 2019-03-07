using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProximitySpawner : MonoBehaviour {
	//public access members
	public GameObject prefab;
	public string targetTag;
	public float minDistance = 4f;
	public float maxDistance = 10f;
	public bool destroyIfTooFar = false;

	//private members
	Transform targetTransform;
	GameObject spawnedObject;

	void Start() {
		targetTransform = GameObject.FindWithTag(targetTag).transform;
	}

	void Update() {
		//get the distance
		float distance = Vector3.Distance(transform.position, targetTransform.position);

		//destroy if the player is too far away
		if (destroyIfTooFar && distance > maxDistance) {
			Destroy(spawnedObject);
			return; //skip out on the next check
		}

		//check the distance, check the spawned object
		if (distance > minDistance && distance < maxDistance && spawnedObject == null) {
			spawnedObject = Instantiate(prefab);
		}
	}
}
