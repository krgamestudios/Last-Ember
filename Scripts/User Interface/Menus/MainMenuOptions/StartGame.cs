using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MenuSystem {
	class StartGame : MenuOption {
		override public void Execute() {
			SceneManager.LoadScene("Gameplay");
		}
	}
}