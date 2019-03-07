using System;
using System.IO;
using UnityEngine;

public class SaveFileManager {
	//public structures
	[Serializable()]
	public class SaveSlot {
		//serializable fields (these can be null, so handle that elsewhere)
		public string currentLocation; //name of the pedestal
		public float secondsPlaying;
		public string awardImage; //TODO: award image

		//abilities
		public bool flameBody;
		public bool chargeSwipe;
		public bool wallSlide;
		public bool flameProjectile;
		public bool flameWings;
	}

	//singleton members
	private static SaveFileManager singletonObject = null;
	public static SaveFileManager Instance {
		get {
			if (singletonObject == null) {
				singletonObject = new SaveFileManager();
			}
			return singletonObject;
		}
	}

	//private internal members
	//

	private SaveFileManager() {
		//
	}

	//public members
	public static string saveSlotFileName; //NOT part of the save structure; only one can be loaded into the game at a time
	public static SaveSlot LoadedSaveSlot { get; set; }

	//methods
	public static SaveSlot LoadData(string fname) {
		if (!File.Exists(fname)) {
			return JsonUtility.FromJson<SaveSlot> ("{}");
		}

		StreamReader streamReader = File.OpenText(fname);
		string jsonString = streamReader.ReadToEnd();
		streamReader.Close();

		return JsonUtility.FromJson<SaveSlot> (jsonString);
	}

	public static void SaveData(SaveSlot save, string fname) {
		string jsonString = JsonUtility.ToJson(save);

		StreamWriter streamWriter = File.CreateText(fname);
		streamWriter.Write(jsonString);
		streamWriter.Close();
	}

	public static SaveSlot CreateBlankSaveSlot() {
		SaveSlot saveSlot = new SaveSlot();

		saveSlot.currentLocation = "Start";
		saveSlot.secondsPlaying = 0f;
		saveSlot.awardImage = "";

		saveSlot.flameBody = false;
		saveSlot.chargeSwipe = false;
		saveSlot.wallSlide = false;
		saveSlot.flameProjectile = false;
		saveSlot.flameWings = false;

		return saveSlot;
	}
}
