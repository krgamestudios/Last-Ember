using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MenuSystem {
	class SwitchMenus : MenuOption {
		public Canvas oldMenu;
		public Canvas newMenu;

		override public void Execute() {
			oldMenu.gameObject.SetActive(false);
			newMenu.gameObject.SetActive(true);
		}
	}
}