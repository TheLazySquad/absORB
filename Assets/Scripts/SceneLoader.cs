using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour {
    public Animator transitionAnimator;
    private float currentLvl;
    private GameObject playerMote;
    public void NextLevel() {
        Time.timeScale = 1;
        int currentLevelInt = int.Parse(SceneManager.GetActiveScene().name);
        int newLevelInt = currentLevelInt + 1;
        if (newLevelInt == 13 || newLevelInt == 25){
            LoadMenu(); Debug.Log("Loading main menu");
        } else {
            string newLevelStr = System.Convert.ToString(newLevelInt);
            LoadScene(newLevelStr);
            Debug.Log("Loading level " + newLevelStr);
        }
    }
    public void LoadMenu(){
        GameObject playerMote = GameObject.Find("Player");
        playerMote.GetComponent<MoteScript>().MenuClosed(true);
    }
    public void LoadScene(string sceneName) {
        GameObject crossfadeObject = GameObject.Find("Crossfade");
        transitionAnimator = crossfadeObject.GetComponent<Animator>();
        StartCoroutine(LoadLevel(sceneName));
    } 
    IEnumerator LoadLevel(string sceneName) {
        transitionAnimator.SetTrigger("StartTransition");
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(sceneName);
    }
}
