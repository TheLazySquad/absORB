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
        Scene currentLvl = SceneManager.GetActiveScene();
        if (currentLvl.name == "R1") {
            LoadScene("R2");
        }
    }
}