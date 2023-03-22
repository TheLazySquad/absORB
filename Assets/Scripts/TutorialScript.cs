using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialScript : MonoBehaviour {
    Color lerpedColor = Color.white;
    public bool timebardisabled = true;
    public bool titleDisabled = false;
    public GameObject titleText;
    public GameObject clickToMove;
    public GameObject tutorialText3;
    public GameObject tutorialText4;
    public GameObject tutorialText5;
    public GameObject tutorialText6;

    void Start() {
        Debug.Log("Tutorial Enabled");
        StartCoroutine(DisableTimebar());
        TutorialEnabled();
    }
    void Update() {
        if (Input.GetMouseButton(0)) {
            if (!titleDisabled) {   
                titleText.SetActive(false);
                clickToMove.SetActive(false);
            }
        }
    }
    void DisableTitleText() {

    }
    IEnumerator DisableTimebar() {
        yield return new WaitForSeconds(1);
        // TimeBar.SetActive(false);
        Debug.Log("Timebar Disabled");
    }
    void TutorialEnabled(){
        Debug.Log("tutorial enabled from tutorialscript");
        
    }
}