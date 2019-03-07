using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveHandler : MonoBehaviour {
	public GameObject playerObject;
	public Dictionary<string, Structures.Pedestal> pedestalDictionary = new Dictionary<string, Structures.Pedestal>();

	public bool saveMenuAvailable = false;

	public float startTime = 0f;

	void Start() {
		//create the save file if it doesn't exist
		if (SaveFileManager.LoadedSaveSlot == null) {
			SaveFileManager.saveSlotFileName = Path.Combine(Application.persistentDataPath, DateTime.Now.ToString("yyyyMMddTHHmmss") + ".sav");
			SaveFileManager.LoadedSaveSlot = SaveFileManager.CreateBlankSaveSlot();

//			SaveFileManager.SaveData(SaveFileManager.LoadedSaveSlot, SaveFileManager.saveSlotFileName);
		}

		//initialize the game world with the given save data
		startTime = Time.time;

		//convert the array into a quick-search dictionary
		Structures.Pedestal[] pedestals = GameObject.FindObjectsOfType<Structures.Pedestal>();
		foreach(Structures.Pedestal pedestal in pedestals) {
			pedestal.SaveHandler = this;
			pedestalDictionary[pedestal.name] = pedestal;
		}

		//place the player on their saved pedestal
		playerObject.transform.position = pedestalDictionary[SaveFileManager.LoadedSaveSlot.currentLocation].gameObject.transform.position;
	}
}
