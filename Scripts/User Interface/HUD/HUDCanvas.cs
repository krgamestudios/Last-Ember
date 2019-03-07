using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDCanvas : MonoBehaviour {
	//public access members
	public Sprite[] sparkSprites;
	public Sprite[] flameSprites;

	//public properties
	int _sparkLevel = 0;
	public int SparkLevel {
		get {
			return _sparkLevel;
		}
		set {
			_sparkLevel = value;
			if (_sparkLevel < 0) {
				_sparkLevel = 0;
			}
			if (_sparkLevel > sparkSprites.Length * (flameSprites.Length - 1)) {
				_sparkLevel = sparkSprites.Length * (flameSprites.Length - 1);
			}
		}
	}
	public int FlameLevel {
		get {
			return _sparkLevel / sparkSprites.Length;
		}
		set {
			_sparkLevel = value * sparkSprites.Length + _sparkLevel % sparkSprites.Length;
			while(_sparkLevel < 0) {
				_sparkLevel += sparkSprites.Length;
			}
			while(_sparkLevel > sparkSprites.Length * (flameSprites.Length - 1)) {
				_sparkLevel -= 1;
			}
		}
	}

	//internal members
	Image[] childImages;

	void Start() {
		childImages = GetComponentsInChildren<Image>();
	}

	void Update() {
		HandleGraphics();
		DebugHandleInput();
	}

	void HandleGraphics() {
		childImages[0].sprite = sparkSprites[SparkLevel % sparkSprites.Length];
		childImages[1].sprite = flameSprites[SparkLevel / sparkSprites.Length];
	}

	void DebugHandleInput() {
		if (Input.GetKeyDown("1")) {
			SparkLevel--;
		}

		if (Input.GetKeyDown("2")) {
			SparkLevel++;
		}

		if (Input.GetKeyDown("3")) {
			FlameLevel--;
		}

		if (Input.GetKeyDown("4")) {
			FlameLevel++;
		}
	}
}
