using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour {
    private float currentLvl;
    public void LoadScene(string sceneName)
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(sceneName);
    }
    public void NextLevel() {
        int currentLevelInt = int.Parse(SceneManager.GetActiveScene().name); // change the string into an integer
        int newLevelInt = currentLevelInt + 1; // add 1 to the current level integer
        if (newLevelInt == 13 || newLevelInt == 25){
            LoadScene("MainMenu");
        } else {
            string newLevelStr = System.Convert.ToString(newLevelInt);
            LoadScene(newLevelStr);
        }
    }
}