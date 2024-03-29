using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TutorialScript : MonoBehaviour {

    // Motes
    public GameObject Mote1;
    public GameObject Mote2;

    // Text Objects
    public GameObject TXT_ClickToMove;
    public GameObject TXT_ScrollToZoom;
    public GameObject TXT_DragScaleTime;
    public GameObject TXT_AbsorbTheMote1;
    public GameObject TXT_AbsorbTheMote2;
    public GameObject TXT_AbsorbTheMote3;
    public GameObject TXT_AbsorbTheMote4;
    
    // Booleans
    private bool freezePlayer = true;
    public bool tutorialCompleted = false;
    private bool clickToMove = true;
    private bool scrollToZoom = false;
    private bool dragScaleTime = false;
    
    // Animator
    public Animator transitionAnimator;
    
    // Other
    private float prevMousePosY;
    private float holdTime;

    void Start() {
        Mote1.SetActive(false);
        Mote2.SetActive(false);
        FastOut(TXT_ClickToMove);
        FastOut(TXT_ScrollToZoom);
        FastOut(TXT_DragScaleTime);
        FastOut(TXT_AbsorbTheMote1);
        FastOut(TXT_AbsorbTheMote2);
        FastOut(TXT_AbsorbTheMote3);
        FastOut(TXT_AbsorbTheMote4);
        StartCoroutine(StartText());
    }
    IEnumerator StartText() {
        yield return new WaitForSecondsRealtime(3);
        FadeIn(TXT_ClickToMove);
    }
    void Update() {
        Progresses();
        InputTypes();
    }
    void Progresses() {
        if (freezePlayer) {
            this.transform.position = new Vector2(0, 0);
        }
        if (clickToMove) {
            if (Input.GetMouseButtonDown(0)) {
                StartCoroutine(ClickedToMove());
                clickToMove = false;
            }
        }
        if (scrollToZoom) {
            if (Input.mouseScrollDelta.y != 0) {
                StartCoroutine(ScrolledToZoom());
                scrollToZoom = false;
            }
        }
        if (dragScaleTime) {
            if (Input.GetMouseButton(0)) {
                holdTime += Time.deltaTime;
            }
            if (holdTime > 0.5f) {
                float currentMousePositionY = Input.mousePosition.y; // set the y position variable to the current y position of the mouse
                if (currentMousePositionY != prevMousePosY) {
                    StartCoroutine(DraggedScaleTime());
                    dragScaleTime = false;
                }
                prevMousePosY = currentMousePositionY;
            }
        }
    }
    void InputTypes() {
        if (Application.platform == RuntimePlatform.Android) {
        // This code will be executed on Android.
        SetText(TXT_ClickToMove, "Tap to move");
        SetText(TXT_DragScaleTime, "Drag up and down to scale time");
        SetText(TXT_ScrollToZoom, "Pinch to zoom in and out");
        }
        else if (Application.platform == RuntimePlatform.WindowsPlayer) {
        // This code will be executed on Windows.
        SetText(TXT_ClickToMove, "Click to move");
        SetText(TXT_DragScaleTime, "Drag the mouse up and down to scale time");
        SetText(TXT_ScrollToZoom, "Scroll to zoom in and out");
        }
    }

    // Coroutines
    IEnumerator ClickedToMove() {
        yield return new WaitForSecondsRealtime(1);
        FadeOut(TXT_ClickToMove);
        yield return new WaitForSecondsRealtime(4);
        FadeIn(TXT_ScrollToZoom);
        yield return new WaitForSecondsRealtime(1);
        scrollToZoom = true;
    }
    IEnumerator ScrolledToZoom() {
        yield return new WaitForSecondsRealtime(1);
        FadeOut(TXT_ScrollToZoom);
        yield return new WaitForSecondsRealtime(4);
        FadeIn(TXT_DragScaleTime);
        yield return new WaitForSecondsRealtime(1);
        dragScaleTime = true;
    }
    IEnumerator DraggedScaleTime() {
        yield return new WaitForSecondsRealtime(1);
        FadeOut(TXT_DragScaleTime);
        yield return new WaitForSecondsRealtime(10);
        Mote1.SetActive(true);
        Mote2.SetActive(true);
        Time.timeScale = 1f;
        freezePlayer = false;
        FadeIn(TXT_AbsorbTheMote1);
        yield return new WaitForSecondsRealtime(5);
        FadeOut(TXT_AbsorbTheMote1);
        yield return new WaitForSecondsRealtime(2);
        FadeIn(TXT_AbsorbTheMote2);
        yield return new WaitForSecondsRealtime(5);
        FadeOut(TXT_AbsorbTheMote2);
        yield return new WaitForSecondsRealtime(2);
        FadeIn(TXT_AbsorbTheMote3);
        yield return new WaitForSecondsRealtime(5);
        FadeOut(TXT_AbsorbTheMote3);
        yield return new WaitForSecondsRealtime(2);
        FadeIn(TXT_AbsorbTheMote4);
        yield return new WaitForSecondsRealtime(5);
        FadeOut(TXT_AbsorbTheMote4);
        yield return new WaitForSecondsRealtime(2);
        tutorialCompleted = true;
    }
    void SetText(GameObject target, string text) {
        Text textcomp = target.GetComponent<Text>();
        textcomp.text = text;
    }
    // Fade in/out
    void FadeIn(GameObject gObj) {
        transitionAnimator = gObj.GetComponent<Animator>();
        transitionAnimator.SetTrigger("fadeIn");
    }
    void FadeOut(GameObject gObj) {
        transitionAnimator = gObj.GetComponent<Animator>();
        transitionAnimator.SetTrigger("fadeOut");
    }
    void FastOut(GameObject gObj) {
        CanvasGroup cgroup = gObj.GetComponent<CanvasGroup>();
        cgroup.alpha = 0;
    }
}