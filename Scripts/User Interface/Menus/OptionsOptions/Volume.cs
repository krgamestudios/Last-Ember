using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace MenuSystem {
	public class Volume : MenuOption {
		//component references
		ConfigurationManager configManager;
		TextMeshProUGUI text;

		void Start() {
			configManager = ConfigurationManager.Instance;
			text = GetComponent<TextMeshProUGUI>();
			configManager.volume = (configManager.volume != null && configManager.volume != 0 ? configManager.volume : 100f);
			text.text = configManager.volume.ToString();

			UpdateMasterVolume();
		}

		public override void Execute() {
			//DO NOTHING
		}

		public override void Scroll(float x) {
			//DO NOTHING
			configManager.volume += x;

			configManager.volume = Mathf.Clamp(configManager.volume, 0f, 100f);

			text.text = configManager.volume.ToString();

			UpdateMasterVolume();
		}

		void UpdateMasterVolume() {
			AudioListener.volume = configManager.volume / 100f;
		}
	}
}