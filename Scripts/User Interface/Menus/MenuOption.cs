using UnityEngine;

namespace MenuSystem {
	public class MenuOption : MonoBehaviour {
		//if this option is being hovered over, logically
		public bool hover = false;

		public virtual void Execute() {
			//DO NOTHING
		}

		public virtual void Scroll(float x) {
			//DO NOTHING
		}
	}
}