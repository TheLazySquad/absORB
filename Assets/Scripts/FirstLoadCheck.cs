using UnityEngine;
using UnityEngine.SceneManagement;

public class FirstLoadCheck : MonoBehaviour
{
    void Start() {
        Debug.Log("IsFirstTimeLoad value: " + PlayerPrefs.GetInt("IsFirstTimeLoad"));
        if ((!PlayerPrefs.HasKey("IsFirstTimeLoad")) || (PlayerPrefs.GetInt("IsFirstTimeLoad") != 1)) { // This code will run on the first-time load
            Debug.Log("First-time load!");
            PlayerPrefs.SetInt("IsFirstTimeLoad", 1); // Set the key to indicate that the game has been loaded at least once
            SceneManager.LoadScene("Tutorial");
        }
    }
}