using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class SaveLoad {

	public static void Save(List<bool[]> fedFile) {
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create (Application.persistentDataPath + "/savedGames.gd");
		bf.Serialize(file,fedFile);
		file.Close ();
	}

	public static void Load(List<bool[]> fedArray) {
		if (File.Exists (Application.persistentDataPath + "/savedGames.gd")) {
			Debug.Log ("SaveData");
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (Application.persistentDataPath + "/savedGames.gd", FileMode.Open);
			List<bool[]> tempArray = (List<bool[]>)bf.Deserialize (file);
			foreach (bool[] aBoolA in tempArray) {
				fedArray.Add (aBoolA);
			}
			file.Close ();
		} else {
			Debug.Log ("NoSaveData");
			bool[] newBA = new bool[]{true,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false};
			List<bool[]> tempBAL = new List<bool[]> ();
			tempBAL.Add (newBA);
			SaveLoad.Save (tempBAL);
			SaveLoad.Load (fedArray);
		}
	}
}