using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIScript : MonoBehaviour {
    private GameObject playerMote;
    
    void OnEnable() {
        playerMote = GameObject.Find("Player");
    }
    public void UIResume() {
        MoteScript playerMoteScript = playerMote.GetComponent<MoteScript>();
        playerMoteScript.MenuClosed(false);
    }
    public void BackToMenu() {
        MoteScript playerMoteScript = playerMote.GetComponent<MoteScript>();
        playerMoteScript.MenuClosed(true);
    }
    public void UIReloadScene() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // using GetActiveScene instead of just using the name of the scene is nice because i made all of the levels their own scene. https://docs.unity3d.com/ScriptReference/SceneManagement.SceneManager.GetActiveScene.        
    }
}
