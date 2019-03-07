using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MenuSystem {
	public class ConfirmLoad : MenuOption {
			//public access members
			public SaveFileManager.SaveSlot saveData;
			public string fileName;

			void Update() {
				if (transform.childCount > 0) {
					//BUGFIX: I don't know why this is disabled here
					transform.GetChild(0).GetComponent<Image>().enabled = true;
				}
			}

			public override void Execute() {
				SaveFileManager.saveSlotFileName = fileName;
				SaveFileManager.LoadedSaveSlot = saveData;
				SceneManager.LoadScene("Gameplay");
			}

			public override void Scroll(float x) {
				//DO NOTHING
			}
	}
}