using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MenuSystem {
	class Quit : MenuOption {
		override public void Execute() {
			Application.Quit();
			Debug.Log("Quit called");
		}
	}
}