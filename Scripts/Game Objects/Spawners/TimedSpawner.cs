using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedSpawner : MonoBehaviour {
	//inspector access elements
	public GameObject prefab;
	public float spawnDelay = 10f;
	public int maxSpawns = 3;

	//private members
	float lastSpawnTime = float.NegativeInfinity;
	List<GameObject> spawnList = new List<GameObject>();

	//TODO: maybe all spawners should shut off (and monsters become inactive) when the player is too far away.
	void Update() {
		//prune the list
		spawnList.RemoveAll(spawn => spawn == null);

		//spawn if enough time has passed
		if (Time.time - lastSpawnTime > spawnDelay && spawnList.Count < maxSpawns) {
			lastSpawnTime = Time.time;

			spawnList.Add(Instantiate(prefab, transform.position, Quaternion.identity, transform));
		}
	}
}
