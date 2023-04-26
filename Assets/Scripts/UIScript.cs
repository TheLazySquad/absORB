using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIScript : MonoBehaviour {
    private GameObject playerMote;
    void OnEnable() { // when this script becomes active
        playerMote = GameObject.Find("Player"); // Find the player object
    }
    public void UIResume() { // publicly callable resume function
        MoteScript playerMoteScript = playerMote.GetComponent<MoteScript>(); // get the script from the player object
        playerMoteScript.MenuClosed(false); // run a function in the player object's script
    }
    public void BackToMenu() { // publicly callable back to the menu function
        MoteScript playerMoteScript = playerMote.GetComponent<MoteScript>(); // get the script from the player object
        playerMoteScript.MenuClosed(true); // run a function in the player object's script
    }
    public void UIReloadScene() { // publicly callable scene reloader
        Time.timeScale = 1f; // set the time scale back to 1 so that the transitions are played at the right speed
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // GetActiveScene is nice because it allowes me to get the build index (.buildindex) from each scene. Each level uses the corrosponding build index, so its easy to jump between levels: e.g. Level 1 has the build index of one, and level 8 has the build index of 8.
        // Build Index refrence: https://docs.unity3d.com/ScriptReference/SceneManagement.Scene-buildIndex.html
    }
}
