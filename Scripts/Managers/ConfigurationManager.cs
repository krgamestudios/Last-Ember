using System;
using System.IO;
using UnityEngine;

[Serializable()]
public class ConfigurationManager {
	//singleton members
	private static ConfigurationManager singletonObject = null;
	public static ConfigurationManager Instance {
		get {
			if (singletonObject == null) {
				string fname = Path.Combine(Application.persistentDataPath, "configuration.json");
				new ConfigurationManager(fname); //NOTE: singleton object assigned elsewhere
			}
			return singletonObject;
		}
	}

	//serializable fields (these can be null, so handle that elsewhere)
	public string textSpeed;
	public float volume;

	//private internal members
	static bool initialized = false; //BUGFIX: stack overflow
	static string dataPath;

	//methods
	private ConfigurationManager(string fname) {
		if (!initialized) {
			initialized = true;
			LoadData(fname);
			dataPath = fname;
		}
	}

	public void CleanUp() {
		SaveData(Instance, dataPath);
	}

	void LoadData(string fname) {
		if (!File.Exists(fname)) {
			singletonObject = JsonUtility.FromJson<ConfigurationManager> ("{}");
			return;
		}

		StreamReader streamReader = File.OpenText(fname);
		string jsonString = streamReader.ReadToEnd();
		streamReader.Close();
		singletonObject = JsonUtility.FromJson<ConfigurationManager> (jsonString);

		//reset statics
		initialized = true;
	}

	void SaveData(ConfigurationManager configMgr, string fname) {
		string jsonString = JsonUtility.ToJson(configMgr);
		StreamWriter streamWriter = File.CreateText(fname);
		streamWriter.Write(jsonString);
		streamWriter.Close();
	}
}
