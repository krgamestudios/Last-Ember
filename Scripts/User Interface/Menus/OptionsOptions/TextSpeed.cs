using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace MenuSystem {
	public class TextSpeed : MenuOption {
		//component references
		ConfigurationManager configManager;
		TextMeshProUGUI text;

		void Start() {
			configManager = ConfigurationManager.Instance;
			text = GetComponent<TextMeshProUGUI>();
			configManager.textSpeed = (configManager.textSpeed != null && configManager.textSpeed != "" ? configManager.textSpeed : "Normal");
			text.text = configManager.textSpeed;
		}

		override public void Execute() {
			switch(configManager.textSpeed) {
				case "Normal":
					configManager.textSpeed = "Slow";
					break;

				case "Slow":
					configManager.textSpeed = "Fast";
					break;

				case "Fast":
					configManager.textSpeed = "Normal";
					break;
			}

			text.text = configManager.textSpeed;
		}
	}
}