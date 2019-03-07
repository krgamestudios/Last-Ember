using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MenuSystem {
	public class HideMeIf : MonoBehaviour {
		public SaveHandler saveHandler;

		void OnEnable() {
			if (!saveHandler.saveMenuAvailable) {
				gameObject.SetActive(false);
			}
		}
	}
}