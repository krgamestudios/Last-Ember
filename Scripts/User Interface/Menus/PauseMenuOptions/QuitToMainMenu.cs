using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MenuSystem {
	public class QuitToMainMenu : MenuOption {
		public override void Execute() {
			SceneManager.LoadScene("MainMenu");
			PauseManager.Instance.Paused = false;
		}
	}
}