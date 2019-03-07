using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager {
	//singleton members
	private static PauseManager singletonObject = null;
	public static PauseManager Instance {
		get {
			if (singletonObject != null) {
				return singletonObject;
			} else {
				return singletonObject = new PauseManager();
			}
		}
		set {
			singletonObject = value;
		}
	}

	//paused controller
	bool paused = false;
	public bool Paused {
		get {
			return paused;
		}
		set {
			paused = value;
			TriggerLists();
		}
	}

	public delegate void CallbackHandler();

	List<CallbackHandler> onPausedList = new List<CallbackHandler>();
	List<CallbackHandler> onResumeList = new List<CallbackHandler>();

	private PauseManager() {}

	public void PushOnPaused(CallbackHandler callback) {
		onPausedList.Add(callback);
	}

	public void PushOnResume(CallbackHandler callback) {
		onResumeList.Add(callback);
	}

	public void PurgeLists() {
		onPausedList.Clear();
		onResumeList.Clear();
	}

	void TriggerLists() {
		if (Paused) {
			foreach(CallbackHandler callback in onPausedList) {
				callback();
			}
		}
		else {
			foreach(CallbackHandler callback in onResumeList) {
				callback();
			}
		}
	}
}
