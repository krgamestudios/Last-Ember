using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace MenuSystem {
	public class ConfirmDelete : MenuOption {
		//public access members
		public string fileName;
		public GameObject loadCanvas;
		public GameObject confirmationCanvas;

		public override void Execute() {
			File.Delete(fileName);
			confirmationCanvas.SetActive(false);
			loadCanvas.SetActive(true);
		}

		public override void Scroll(float x) {
			//DO NOTHING
		}
	}
}