using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagerController : MonoBehaviour {
	public delegate void CallbackHandler(Collider2D collider);

	List<CallbackHandler> onTriggerEnter = new List<CallbackHandler>();
	List<CallbackHandler> onTriggerStay= new List<CallbackHandler>();
	List<CallbackHandler> onTriggerExit = new List<CallbackHandler>();

	//public access members
	public void PushOnTriggerEnter(CallbackHandler callback) {
		onTriggerEnter.Add(callback);
	}

	public void PushOnTriggerStay(CallbackHandler callback) {
		onTriggerStay.Add(callback);
	}

	public void PushOnTriggerExit(CallbackHandler callback) {
		onTriggerExit.Add(callback);
	}

	public void PurgeLists() {
		onTriggerEnter.Clear();
		onTriggerStay.Clear();
		onTriggerExit.Clear();
	}

	//monobehaviour members
	void OnTriggerEnter2D(Collider2D collider) {
		foreach(CallbackHandler callback in onTriggerEnter) {
			callback(collider);
		}
	}

	void OnTriggerStay2D(Collider2D collider) {
		foreach(CallbackHandler callback in onTriggerStay) {
			callback(collider);
		}
	}

	void OnTriggerExit2D(Collider2D collider) {
		foreach(CallbackHandler callback in onTriggerExit) {
			callback(collider);
		}
	}
}
