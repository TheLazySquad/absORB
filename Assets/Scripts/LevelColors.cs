using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelColors : MonoBehaviour {
    public int Level; // level number used for finding the corresponding boolean statment in LevelSaver.levelList
    private Renderer rend; // This is used to change the color of the levels.
    public Material blueMat; // material for the completed levels
    public Material redMat; // material for the incomplete levels
    void Start() {
        rend = GetComponent<Renderer>();
    }
    void Update() {
        if (LevelSaver.levelList[Level] == true) { // if the level is completed
            rend.material = blueMat;
        } else if (LevelSaver.levelList[Level] == false) { // if the level is not solved
            rend.material = redMat;
        }
    }
}
