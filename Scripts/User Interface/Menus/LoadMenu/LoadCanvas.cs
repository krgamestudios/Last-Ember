using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace MenuSystem {
	public class LoadCanvas : MonoBehaviour {
		//public access members
		public GameObject saveSlotPrefab;
		public GameObject confirmationCanvas;

		void OnEnable() {
			//load the save objects
			DirectoryInfo info = new DirectoryInfo(Application.persistentDataPath);

			//create the load menu
			foreach(var file in info.GetFiles("*.sav")) {
				GameObject saveSlot = Instantiate(saveSlotPrefab) as GameObject;
				saveSlot.transform.SetParent(transform); //BUGFIX: unity bug
				saveSlot.GetComponent<SaveSlot>().SetSaveSlotInfo(SaveFileManager.LoadData(file.FullName));

				saveSlot.GetComponent<SaveSlot>().fileName = file.FullName;
				saveSlot.GetComponent<SaveSlot>().loadCanvas = transform.gameObject;
				saveSlot.GetComponent<SaveSlot>().confirmationCanvas = confirmationCanvas;
			}

			//Move the "back" option to the bottom
			transform.GetChild(0).SetAsLastSibling();
		}

		void OnDisable() {
			foreach (Transform child in transform) {
				if (child.GetComponent<SaveSlot>() != null) {
					GameObject.Destroy(child.gameObject);
				}
			}
		}
	}
}