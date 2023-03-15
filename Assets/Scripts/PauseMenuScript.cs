using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuScript : MonoBehaviour
{
    public GameObject pauseMenu;
    void Start()
    {
        pauseMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Debug.Log("ESC pressed");
            MenuOpened();
        }
    }
    void MenuOpened() {
        pauseMenu.SetActive(true);
    }
    void MenuClosed() {
        pauseMenu.SetActive(false);
    }
}
