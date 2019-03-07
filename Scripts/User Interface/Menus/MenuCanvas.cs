using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MenuSystem {
	public class MenuCanvas : MonoBehaviour {
		//public access members
		public Image cursorImage;
		public bool floating = false;

		//private members
		MenuOption[] menuOptions = null;

		//fine-tuned input
		const float deadZone = 0.25f;
		const float inputDelay = 0.5f;
		const float scrollDelay = 0.1f;
		float lastInputX = float.NegativeInfinity;
		float lastInputY = float.NegativeInfinity;

		void OnEnable() {
			menuOptions = GetComponentsInChildren<MenuOption>();

			//set up the cursor
			menuOptions[0].hover = true;
			RectTransform rt = cursorImage.GetComponent<RectTransform>();
			rt.SetParent(menuOptions[0].transform);
			rt.offsetMin = new Vector2(0, 0);
			rt.offsetMax = new Vector2(0, 0);

			HandleGraphics();
		}

		void OnDisable() {
			//BUGFIX: reset this menu tree if closing the menu
			foreach (Transform child in transform) {
				child.gameObject.SetActive(true);
			}
		}

		void Update() {
			//BUGFIX: remove all non-active elements
			 menuOptions = Array.FindAll(menuOptions, (MenuOption option) => option.gameObject.active);

			HandleInput();
			HandleGraphics();
		}

		void HandleInput() {
			//press a button
			if (GamePad.GetState().Pressed(CButton.A)) {
				foreach(MenuOption option in menuOptions) {
					if (option.hover) {
						option.Execute();
					}
				}
				return;
			}

			//scroll left or right
			if (Mathf.Abs(GamePad.GetAxis(CAxis.LX)) >= deadZone && Time.unscaledTime - lastInputX > scrollDelay) {
				lastInputX = Time.unscaledTime;

				foreach(MenuOption option in menuOptions) {
					if (option.hover) {
						option.Scroll(GamePad.GetAxis(CAxis.LX));
					}
				}
				return;
			}

			//keyboard/gamepad scroll up
			if (GamePad.GetAxis(CAxis.LY) < -deadZone && Time.unscaledTime - lastInputY > inputDelay) {
				lastInputY = Time.unscaledTime;

				//move the cursor one up
				for (int i = 1; i < menuOptions.Length; i++) {
					//find the current hovered item
					if (menuOptions[i].hover) {
						//move the hover up
						menuOptions[i].hover = false;
						menuOptions[i-1].hover = true;
						break;
					}
				}
			}

			//keyboard/gamepad scroll down
			if (GamePad.GetAxis(CAxis.LY) > deadZone && Time.unscaledTime - lastInputY > inputDelay) {
				lastInputY = Time.unscaledTime;

				//move the cursor one down
				for (int i = 0; i < menuOptions.Length - 1; i++) {
					//find the current hovered item
					if (menuOptions[i].hover) {
						//move the hover down
						menuOptions[i].hover = false;
						menuOptions[i+1].hover = true;
						break;
					}
				}
			}

			//deadzone reset lastInputX
			if (Mathf.Abs(GamePad.GetAxis(CAxis.LX)) < deadZone) {
				lastInputX = float.NegativeInfinity;
			}

			//deadzone reset lastInputY
			if (Mathf.Abs(GamePad.GetAxis(CAxis.LY)) < deadZone) {
				lastInputY = float.NegativeInfinity;
			}
		}

		void HandleGraphics() {
			bool hoverSet = false;

			//update the graphics
			foreach(MenuOption option in menuOptions) {
				if (option.hover) {
					if (hoverSet) {
						option.hover = false;
						continue;
					}

					RectTransform rt = cursorImage.GetComponent<RectTransform>();
					rt.SetParent(option.transform);
					rt.anchorMin = new Vector2(0, 0);
					rt.anchorMax = new Vector2(1, 1);
					rt.offsetMin = new Vector2(0, 0);
					rt.offsetMax = new Vector2(0, 0);
					hoverSet = true;
				}
			}

			if (floating) {
				float offset = cursorImage.transform.position.y - Screen.height / 2;
				transform.position = new Vector2(transform.position.x, transform.position.y - offset);
			}
		}
	}
}