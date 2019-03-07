using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;

namespace MenuSystem {
	public class Heading : MonoBehaviour {
		void Start() {
			TextMeshProUGUI text = GetComponent<TextMeshProUGUI>();

			if (!File.Exists(SaveFileManager.saveSlotFileName)) {
				text.text = "Save The Game?";
			}
		}
	}
}