using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class LevelSaver : MonoBehaviour {
    public static bool lvl0; // tutorial level
    
    //red levels
    public static bool lvl1; public static bool lvl2; public static bool lvl3; public static bool lvl4; public static bool lvl5; public static bool lvl6; public static bool lvl7; public static bool lvl8; public static bool lvl9; public static bool lvl10; public static bool lvl11; public static bool lvl12;
    //yellow levels
    public static bool lvl13; public static bool lvl14; public static bool lvl15; public static bool lvl16; public static bool lvl17; public static bool lvl18; public static bool lvl19; public static bool lvl20; public static bool lvl21; public static bool lvl22; public static bool lvl23; public static bool lvl24;
    
    public static List<bool> levelList;
    private string filePath;
    private bool levelListChanged = false;
    private void Start() {
        levelList = new List<bool>() {lvl0, lvl1, lvl2, lvl3, lvl4, lvl5, lvl6, lvl7, lvl8, lvl9, lvl10, lvl11, lvl12, lvl13, lvl14, lvl15, lvl16, lvl17, lvl18, lvl19, lvl20, lvl21, lvl22, lvl23, lvl24}; // list of all levels
        filePath = Application.persistentDataPath + "/GameData.json"; // Set the file path to "GameData.json"
        if (File.Exists(filePath)) {// If the file already exists, load the lvlList from the file
            string jsonData = File.ReadAllText(filePath);
            levelList = JsonUtility.FromJson<ListWrapper>(jsonData).lvlList;
            UpdateFileList();
        } else {SaveLvlList();} // If the file doesn't exist, create it and save the default values of lvlList to it 
    }

    private void OnApplicationQuit() {SaveLvlList();} // When the application quits, save the current values of lvlList to the file

    private void SaveLvlList() {
        string jsonData = JsonUtility.ToJson(new ListWrapper(levelList)); // convert to JSON with ListWrapper because without ListWrapper, the game does not know what kind of data the file is storing or what to do with it.
        File.WriteAllText(filePath, jsonData); // write to file
    }

    // Wrapper class for serializing and deserializing a list of bools
    public class ListWrapper {
        public List<bool> lvlList;
        public ListWrapper(List<bool> lvlList) {this.lvlList = lvlList;}
    }

    private void UpdateFileList() { // set the boolen values to the values of the level list
        lvl0 = levelList[0];
        lvl1 = levelList[1];
        lvl2 = levelList[2];
        lvl3 = levelList[3];
        lvl4 = levelList[4];
        lvl5 = levelList[5];
        lvl6 = levelList[6];
        lvl7 = levelList[7];
        lvl8 = levelList[8];
        lvl9 = levelList[9];
        lvl10 = levelList[10];
        lvl11 = levelList[11];
        lvl12 = levelList[12];
        lvl13 = levelList[13];
        lvl14 = levelList[14];
        lvl15 = levelList[15];
        lvl16 = levelList[16];
        lvl17 = levelList[17];
        lvl18 = levelList[18];
        lvl19 = levelList[19];
        lvl20 = levelList[20];
        lvl21 = levelList[21];
        lvl22 = levelList[22];
        lvl23 = levelList[23];
        lvl24 = levelList[24]; 
    }
    private void UpdateLevelList() { // set the list values to the boolean values
        levelList[0] = lvl0;
        levelList[1] = lvl1;
        levelList[2] = lvl2;
        levelList[3] = lvl3;
        levelList[4] = lvl4;
        levelList[5] = lvl5;
        levelList[6] = lvl6;
        levelList[7] = lvl7;
        levelList[8] = lvl8;
        levelList[9] = lvl9;
        levelList[10] = lvl10;
        levelList[11] = lvl11;
        levelList[12] = lvl12;
        levelList[13] = lvl13;
        levelList[14] = lvl14;
        levelList[15] = lvl15;
        levelList[16] = lvl16;
        levelList[17] = lvl17;
        levelList[18] = lvl18;
        levelList[19] = lvl19;
        levelList[20] = lvl20;
        levelList[21] = lvl21;
        levelList[22] = lvl22;
        levelList[23] = lvl23;
        levelList[24] = lvl24;

        levelListChanged = true; // kind of announce that the level list has changed so that we can save the changes
    }
    void Update() {
        // Check if any of the level bool values have changed
        if (lvl0 != levelList[0] || lvl1 != levelList[1] || lvl2 != levelList[2] || lvl3 != levelList[3] || lvl4 != levelList[4] || lvl5 != levelList[5] || lvl6 != levelList[6] || lvl7 != levelList[7] || lvl8 != levelList[8] || lvl9 != levelList[9] || lvl10 != levelList[10] || lvl11 != levelList[11] || lvl12 != levelList[12] || lvl13 != levelList[13] || lvl14 != levelList[14] || lvl15 != levelList[15] || lvl16 != levelList[16] || lvl17 != levelList[17] || lvl18 != levelList[18] || lvl19 != levelList[19] || lvl20 != levelList[20] || lvl21 != levelList[21] || lvl22 != levelList[22] || lvl23 != levelList[23] || lvl24 != levelList[24]) {
            UpdateLevelList(); // if anything has changed, save the changes
        }
        if (levelListChanged) {
            SaveLvlList(); // If levelListChanged is true, save the updated levelList to the file
            levelListChanged = false; // reset the change checker
        }
    }
}
/* References:
Persistant data / filePath: https://docs.unity3d.com/ScriptReference/Application-persistentDataPath.html
Deserializing JSON Arrays: https://forum.unity.com/threads/cannot-deserialize-json-object.778688/#post-5182715
*/