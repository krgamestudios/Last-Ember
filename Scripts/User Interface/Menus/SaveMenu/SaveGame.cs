using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MenuSystem {
	public class SaveGame : MenuOption {
		public SaveHandler saveHandler;

		public Canvas oldMenu;
		public Canvas newMenu;

		public override void Execute() {
			//calculate playtime
			SaveFileManager.LoadedSaveSlot.secondsPlaying += Time.time - saveHandler.startTime;
			saveHandler.startTime = Time.time;

			//save the game
			SaveFileManager.SaveData(SaveFileManager.LoadedSaveSlot, SaveFileManager.saveSlotFileName);
			oldMenu.gameObject.SetActive(false);
			newMenu.gameObject.SetActive(true);
		}

		public override void Scroll(float x) {
			//DO NOTHING
		}
	}
}