using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Structures {
	public class Pedestal : MonoBehaviour {
		public string name;
		public SaveHandler SaveHandler { set; get; }

		void OnCollisionEnter2D(Collision2D collision) {
			if (collision.gameObject.tag == "Player") {
				SaveFileManager.LoadedSaveSlot.currentLocation = name;
				SaveHandler.saveMenuAvailable = true;
			}
		}

		void OnCollisionExit2D(Collision2D collision) {
			if (collision.gameObject.tag == "Player") {
				SaveHandler.saveMenuAvailable = false;
			}
		}
	}
}