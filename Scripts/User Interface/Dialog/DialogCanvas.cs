using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DialogSystem {
	public class DialogCanvas : MonoBehaviour {
		//public access references
		public Image dialogPanel;
		public TextMeshProUGUI dialogText;

		//internal variables
		bool visible;
		List<string> internalTextList;

		//fine-tuning the interface
		float speed = 0.25f;
		float speedCharCount = 0;

		void Start() {
			SetVisible(false);
		}

		void Update() {
			if (!PauseManager.Instance.Paused && visible) {
				HandleInput();
			}
		}

		void FixedUpdate() {
			if (visible) {
				ProcessSpeed();
				ProcessText();
			}
		}

		public void SetVisible(bool b) {
			visible = b;

			dialogPanel.gameObject.SetActive(visible);
			dialogText.gameObject.SetActive(visible);
		}

		public bool GetVisible() {
			return visible;
		}

		public void SetText(List<string> stringList, bool setVisible = true) {
			internalTextList = stringList;
			dialogText.text = "";
			SetVisible(setVisible);
		}

		void HandleInput() {
			if (GamePad.GetState().Pressed(CButton.A)) {
				if (internalTextList[0].Length > speedCharCount) {
					//skip the text scroll
					speedCharCount = internalTextList[0].Length;
				} else {
					//skip to the next "page"
					internalTextList.RemoveAt(0);
					speedCharCount = 0;

					//if there isn't another "page", go dormant
					if (internalTextList.Count == 0) {
						SetVisible(false);
					}
				}
			}
		}

		void ProcessSpeed() {
			switch(ConfigurationManager.Instance.textSpeed) {
				case "Fast":
					speed = 1f;
					break;

				case "Normal":
					speed = 0.25f;
					break;

				case "Slow":
					speed = 0.1f;
					break;
			}
		}

		void ProcessText() {
			//if the list of text has run out
			if (internalTextList.Count == 0) {
				SetVisible(false);
				return;
			}

			SetVisible(true);

			//only show the first "speedCharCount" characters
			string thisLine = internalTextList[0];
			if (speedCharCount >= thisLine.Length) {
				dialogText.text = thisLine;
				return;
			}
			speedCharCount += speed;
			thisLine = thisLine.Substring(0, (int)Mathf.Floor(speedCharCount));

			dialogText.text = thisLine;
		}
	}
}