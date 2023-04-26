using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour {
    public Animator transitionAnimator;
    private float currentLvl;
    private GameObject playerMote;
    public void NextLevel() { // load the next level function featuring lots of fun string & integer stuff
        Time.timeScale = 1; // set the time scale back to 1 so the cool crossfade animations play at the right speed
        string currentLvlStr = SceneManager.GetActiveScene().name; // make the current level into a string we can use
        if (currentLvlStr == "Tutorial") { // if you've just beat the tutorial
            LoadScene("MainMenu"); // load the main menu
        }else {
            int currentLevelInt = int.Parse(SceneManager.GetActiveScene().name); // string to int with parse
            int newLevelInt = currentLevelInt + 1; // add 1 to the current level number
            if (newLevelInt == 13 || newLevelInt == 25){ // if youve beat a section of the game go back to the main menu
                LoadMenu();
            } else { // otherwise convert back to string and load the new level
                string newLevelStr = System.Convert.ToString(newLevelInt);
                LoadScene(newLevelStr);
                Debug.Log("Loading level " + newLevelStr);
            }
        }
    }
    public void LoadMenu(){ // publicly callable back-to-menu function
        Time.timeScale = 1; // don't forget to set the time back to normal for those cool transitions
        GameObject playerMote = GameObject.Find("Player"); // get the player object
        playerMote.GetComponent<MoteScript>().MenuClosed(true); // call the menu closing function and let the player object know that we are loading the menu next by passing in the boolean value
    }
    public void LoadScene(string sceneName) { // load a scene via string
        GameObject crossfadeObject = GameObject.Find("Crossfade"); // get the object that is crossfade
        transitionAnimator = crossfadeObject.GetComponent<Animator>(); // get the animator on the crossfade
        StartCoroutine(LoadLevel(sceneName)); // start the epic loading coroutine
    } 
    IEnumerator LoadLevel(string sceneName) { // Load level Coroutine to wait for the cool animations that took forever to play out before leaving the scene
        transitionAnimator.SetTrigger("fadeIn"); // fade in and out are backwards from what you think they should be because we are fading in a black screen, not fading out all the objects
        yield return new WaitForSecondsRealtime(2); /// wait for 2 seconds because my transition animations take two seconds
        SceneManager.LoadScene(sceneName); // actualy do the physical loading
    }
}
// I promise the transitions are cool and worth making this very simple task its own script with a bunch of different functions