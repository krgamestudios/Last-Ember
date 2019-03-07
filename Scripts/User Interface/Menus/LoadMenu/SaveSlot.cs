using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace MenuSystem {
	public class SaveSlot : MenuOption {
		public string fileName;
		public GameObject loadCanvas;
		public GameObject confirmationCanvas;

		//private access members
		SaveFileManager.SaveSlot saveData;

		public void SetSaveSlotInfo(SaveFileManager.SaveSlot saveSlot) {
			TextMeshProUGUI[] texts = GetComponentsInChildren<TextMeshProUGUI>(true);

			texts[0].text = saveSlot.currentLocation;

			TimeSpan time = TimeSpan.FromSeconds(saveSlot.secondsPlaying);
			texts[1].text = time.ToString(@"hh\:mm\:ss");

			saveData = saveSlot;
		}

		public override void Execute() {
			//show a confirmation canvas
			confirmationCanvas.transform.GetChild(0).GetComponent<ConfirmLoad>().saveData = saveData;
			confirmationCanvas.transform.GetChild(0).GetComponent<ConfirmLoad>().fileName = fileName;
			confirmationCanvas.transform.GetChild(1).GetComponent<ConfirmDelete>().fileName = fileName;

			//BUGFIX: move the cursor somewhere safe
			transform.Find("Cursor").SetParent(confirmationCanvas.transform.GetChild(0));

			//BUGFIX: wait a frame to show the confirmation page, to make sure the cursor is safe
			StartCoroutine(DoStuffNextFrame());
		}

		IEnumerator DoStuffNextFrame() {
			yield return null;
			loadCanvas.SetActive(false);
			confirmationCanvas.SetActive(true);
		}

		public override void Scroll(float x) {
			//DO NOTHING
		}
	}
}