using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MenuSystem {
	public class ReturnToGame : MenuOption {
		public override void Execute() {
			PauseManager.Instance.Paused = false;
		}
	}
}