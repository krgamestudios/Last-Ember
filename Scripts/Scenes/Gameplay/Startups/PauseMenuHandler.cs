using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Startups {
	public class PauseMenuHandler : MonoBehaviour {
		public Canvas pauseMenuCanvas;
		public Canvas optionsMenuCanvas;
		public Canvas saveMenuCanvas;

		PauseManager pauseManager;

		void Start() {
			pauseManager = PauseManager.Instance;
			pauseManager.Paused = false;

			pauseManager.PushOnPaused(() => {
				pauseMenuCanvas.gameObject.SetActive(true);
				Time.timeScale = 0f;
			});

			pauseManager.PushOnResume(() => {
				pauseMenuCanvas.gameObject.SetActive(false);
				optionsMenuCanvas.gameObject.SetActive(false);
				saveMenuCanvas.gameObject.SetActive(false);
				Time.timeScale = 1f;
			});
		}

		void OnDestroy() {
			pauseManager.PurgeLists();
		}

		void Update() {
			if (GamePad.GetState().Pressed(CButton.Start)) {
				pauseManager.Paused = !pauseManager.Paused;
			}
		}
	}
}