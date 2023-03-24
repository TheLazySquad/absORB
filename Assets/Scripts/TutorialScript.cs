using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;    
using Cinemachine;
using Cinemachine.Utility;

public class TutorialScript : MonoBehaviour {
    public GameObject vcam;
    float scale;
    public bool timebardisabled = true;
    public bool titleDisabled = false;
    public GameObject titleText;
    public GameObject clickToStart;

    void Start() {
        scale = 1f;
        vcam.SetActive(false);

    }
    void Update() {
        Debug.Log(scale);
        if (Input.GetMouseButton(0)) {
            titleDisabled = true;
        }
        if (titleDisabled) {
            DisableUIText(titleText);
            DisableUIText(clickToStart);
            // vcam.SetActive(true);
        }
    }
    void DisableUIText(GameObject gObj) {
        Debug.Log(gObj.name + "Should be shrinking");
        if (scale > 0.1) {
            scale -= 0.1f;
            gObj.transform.localScale = new Vector3(scale, scale, scale);
        } else if (scale <= 0.1) { gObj.SetActive(false); }
        
    }
}