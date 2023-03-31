using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

public class MoteScript : MonoBehaviour {
    Renderer rend;
    List<MoteScript> Motes;

    // Universal mote variables
    public bool IsPlayer; // Check if the mote is a player mote
    public Rigidbody2D rb;
    public float moteSize = 1.5f; // The size of the mote, used to calculate the actual size of the mote when the game starts
    
    // Variables for Gravity
    private bool enableGravity = true;
    private float GConstant = 0.0005f; // The gravitational constant for the attraction between all motes

    // Variables for OutOfBounds()
    private float OOBDrag = 0.01f; // The value of the drag force applied by the rigidbody when the mote is outside the set boundaries defined below
    private float OOBForce = 0.1f; // The amount of force applied to the rigidbody when the mote is outside the set boundaries defined below
    private float xBoundary = 20; private float yBoundary = 20; // The coordinates used in OutOfBounds()
    
    // Player mote specific variables
    public GameObject motePrefab; // Reference to the mote prefab
    private float moteSpawnDistance = 0.15f; // The distance to spawn the cloned mote away from the player mote
    private float moteSpawnSize = 0.2f; // The size to set the cloned mote to
    private float moteSpawnForce = 0.02f; // The amount of force to apply to the cloned mote
    private float playerLaunchForce = 1f; // Originally I was using the moteSpawnForce to push the player mote away with the same force - like how equal and opposite reactions work in real life - but that didn't give me the effect I was looking for
    
    // Pause and Time
    private bool Paused = false;
    private bool allowTimeScaler = true;
    private float gameSpeedTime = 1f;
    private float prevMousePosY;
    private bool dragging;

    // Variables for Color()
    public Material bluemat; // Blue material used to designate motes that the player can absorb
    public Material redmat; // Red material used to designate motes that the player cannot absorb
    public GameObject playerMote; // gets the player mote for Color()
    private float playerMoteSize; // the variable for the size of the player gameobject for Color()

    // Variables for CinemachineZoom()
    private bool allowZoom = true;
    private float sizeChangeAmount = 0.5f; // The amount to change the camera's size by
    private float zoomSpeed = 2f; // The speed at which to zoom in/out
    private CinemachineVirtualCamera virtualCamera; // The virtual camera to zoom
    private float targetSize = 5f; // The target size of the camera

    //Variables for winning/losing
    private bool biggerThanPlayer = false;
    private bool playerIsBiggest = false;
    private float biggestMoteSize; // The biggest mote size;
    
    // Variables for UI
    public GameObject Canvas;
    private bool allowMenus = true;
    private bool uiSpawned = false;
    public GameObject pauseMenu;
    public GameObject PlayerTooSmallUI;
    public GameObject PlayerAbsorbedUI;
    public GameObject LvlCompleteUI;

    void OnEnable() {
        MotesList.Motes.Add(this); // Add the mote to the list of motes
        Motes = MotesList.Motes; // Reference to the motes list
        
        if (IsPlayer) { // if we don't check for a player, every time a new mote is spawned, the camera resets itself and gives you virtual wiplash
            // Check for the virtual camera so we can zoom in/out with it in CinemachineZoom()
            virtualCamera = GameObject.FindGameObjectWithTag("vcam").GetComponent<CinemachineVirtualCamera>(); // get the virtual camera component
            if(virtualCamera == null) { // check for null
                Debug.LogError("Where's the virtual camera? I can't find it! Check the tag");
                enabled = false; 
                return;
            } else { // if the camera isn't null
                virtualCamera.m_Lens.OrthographicSize = 5; // The size of the orthographic camera. 5 seems to be a reasonable default
                targetSize = virtualCamera.m_Lens.OrthographicSize; 
            }

            if (Motes == null){
                Motes = new List<MoteScript>(); // Create the list if it hasn't been created already.
                Motes.Add(this);    
            }
        }
    }

    void OnDisable() {
        Motes.Remove(this); // If the mote this script is attached to is disabled, then remove it from the list
    }

    void FixedUpdate() {
        OutOfBounds(); // pushes the motes back to the center of the scene if they are out of bounds

        //Gravity
        if (enableGravity) {
            foreach (MoteScript attractor in Motes) {
                if (attractor != this) {
                    Gravity(attractor);
                }
            }
        }
    }
    void Update() {
        // int numMotes = Motes.Count;
        // Debug.Log("Number of motes: " + numMotes);
        transform.localScale = new Vector3(moteSize * 0.01f, moteSize * 0.01f, 1f); // Calculates the size of the sprite based on the variable moteSize
        Canvas = GameObject.Find("Canvas"); // Find the UI canvas
        playerMote = GameObject.Find("Player"); // Find the gameobject with the IsPlayer flag set to true

        if(IsPlayer){
            Player(); // I put the player stuff under Update instead of FixedUpdate because when it was under FixedUpdate, it would spawn a new mote every frame that the mouse was pressed down because its state gets refreshed every frame and thought it was getting pressed each frame instead of held from when it was initally pressed
            TimeScaler();
            BiggestFinder();
            CinemachineZoom(); // its all the way down at the bottom. have fun scrolling :)
        }
        if (!IsPlayer) {
            Color(); // Change the color based on player size
        }
    }
    void BiggestFinder(){
        playerMoteSize = this.moteSize;
        playerIsBiggest = Motes.TrueForAll(x => !x.biggerThanPlayer);
        if (playerIsBiggest) {
            LvlComplete();
        }
        // TrueForAll: https://stackoverflow.com/questions/17897728/how-to-use-trueforall#:~:text=bool%20alltrue%20%3D%20listOfBools.TrueForAll(b%20%3D%3E%20b)%3B
    } 
    void LvlComplete() {
        Debug.Log("lvlcomplete");
        Time.timeScale = 1;
        allowTimeScaler = false;
        allowMenus = false;
        if (!uiSpawned) {
            uiSpawned = true;
            Instantiate(LvlCompleteUI, Canvas.transform.position, Quaternion.identity, Canvas.transform);
        }
    }
    void Player() {
        if(moteSize > (1.5f * moteSpawnSize)) { // Makes sure that the player mote can't get too small
            if (Input.GetKeyDown(KeyCode.Escape)) { // Check if the escape key is pressed
                if (Paused) {
                    MenuClosed();
                } else if (!Paused) {
                    MenuOpened();
                }
            }
            if(!Paused){
                if (Input.GetMouseButtonDown(0)) { // Check if the left mouse button is pressed down --> https://docs.unity3d.com/ScriptReference/Input.GetMouseButtonDown.html
                    
                    // Rotates the player mote such that its x axis is always facing the mouse
                    Vector3 mousePos = Input.mousePosition;
                    Vector3 objectPos = Camera.main.WorldToScreenPoint (transform.position);
                    mousePos.x = mousePos.x - objectPos.x;
                    mousePos.y = mousePos.y - objectPos.y;
                    float angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
                    // I figured out how to rotate the gameobject twords the mouse from the following link: https://answers.unity.com/questions/10615/rotate-objectweapon-towards-mouse-cursor-2d.html

                    // Spawns a new mote and sets the size
                    Vector3 spawnPosition = transform.position + (transform.right * ((moteSize*0.1f)+moteSpawnDistance));
                    GameObject newMote = Instantiate(motePrefab, spawnPosition, Quaternion.identity); // Spawns the new mote with the spawnPosition
                    MoteScript moteScript = newMote.GetComponent<MoteScript>(); // Gets the script componenet of the gameobject
                    moteScript.moteSize = moteSpawnSize; // Sets the moteSize float of that script to the moteSpawnSize float from this script

                    // Apply force to the new mote
                    Vector2 forceDirection = (mousePos - transform.position).normalized; // Determines the direction to apply forces and normalizes which turns it into a unit vector
                    Rigidbody2D moteRigidbody = newMote.GetComponent<Rigidbody2D>(); // Gets the Rigidbody of the cloned mote so I can apply force to it
                    moteRigidbody.AddForce(forceDirection * moteSpawnForce, ForceMode2D.Impulse); // Add force to the rigidbody in the direction determined by forceDirection

                    // Apply force to the player mote's rigidbody and decrease the size of the player mote
                    rb.AddForce((-forceDirection * playerLaunchForce), ForceMode2D.Force);
                    moteSize -= moteSpawnSize;

                    // References for ForceMode2D: https://docs.unity3d.com/ScriptReference/ForceMode2D.html
                    // I chose to use Impulse on the cloned mote because it dosen't need to accumulate any speed from forces while the player mote does.
                }
            }
        } else {
            PlayerTooSmall();
        }
        // if (this.gameObject.GetComponent<TutorialScript>().isActiveAndEnabled == true) {}
    }
    public void MenuOpened() {
        if (allowMenus) {
            Instantiate(pauseMenu, Canvas.transform.position, Quaternion.identity, Canvas.transform);
            Time.timeScale = 0f; 
            Paused = true;
        }
    }
    public void MenuClosed() {
        Destroy(GameObject.FindGameObjectWithTag("PauseMenu"));
        Time.timeScale = gameSpeedTime;
        Paused = false;
    }
    void PlayerTooSmall() {
        if (allowMenus) {
            playerMote.SetActive(false);
            if (!uiSpawned) {
                uiSpawned = true;
                Instantiate(PlayerTooSmallUI, Canvas.transform.position, Quaternion.identity, Canvas.transform);
            }
        }
    }
    void PlayerMoteAbsorbed() {
        if (allowMenus) {
            if (!uiSpawned) {
                uiSpawned = true;
                Instantiate(PlayerAbsorbedUI, Canvas.transform.position, transform.rotation, Canvas.transform);
            }
        }
    }
    void OutOfBounds() { // Defines what will happen when the mote is past a certain distance (xBoundary & yBoundary) in each direction.
        if (-xBoundary > rb.transform.position.x) {
            rb.AddForce(Vector2.right*OOBForce);
            rb.drag = OOBDrag;
        } else if (xBoundary < rb.transform.position.x) {
            rb.AddForce(Vector2.left*OOBForce);
            rb.drag = OOBDrag;
        } else if (-yBoundary > rb.transform.position.y) {
            rb.AddForce(Vector2.up*OOBForce);
            rb.drag = OOBDrag;
        } else if (yBoundary < rb.transform.position.y) {
            rb.AddForce(Vector2.down*OOBForce);
            rb.drag = OOBDrag;
        } else {
            rb.drag = 0.00f; // Universal drag constant. All motes will always feel this drag on their rigidbody while inside the defined boundaries. I don't want to set it to a variable because there is no reason it should ever not be 0 unless I want to change how the entire game feels
        }
    }
    void Gravity(MoteScript affectedObject) { // Calculate and apply a simulated gravitational force to all motes
        Vector2 direction = rb.position - affectedObject.rb.position; // Calculates the direction of the gravity by subtracting the position of the affected rigidbody from the current position
        float distance = direction.magnitude; // the distance is the magnitude of the direction vector
        float forceStrength = GConstant * (rb.mass * affectedObject.rb.mass) / Mathf.Pow(distance, 2); // Gravitational Force = The Gravitational Constant x First Object's Mass x Second Object's Mass / Distance between the two objects squared.  
        affectedObject.rb.AddForce(direction.normalized * forceStrength); // Applying "gravity" force
    }

    void Absorb(MoteScript otherMote) { // The Absorb function basically eats the smaller object when they collide
        if (moteSize > otherMote.moteSize) { // Absorb the other mote if this mote is bigger
            if (otherMote.name == playerMote.name) { // If the other mote is not the player mote
                moteSize += otherMote.moteSize;
                Destroy(otherMote.gameObject);
                PlayerMoteAbsorbed();
            } else if (otherMote.name != playerMote.name) { // If the other mote is the player mote
                moteSize += otherMote.moteSize;
                Destroy(otherMote.gameObject);
            }
        } else if (moteSize == otherMote.moteSize){
            if (otherMote.moteSize == moteSize) {
                if (otherMote.name == playerMote.name) {
                    PlayerMoteAbsorbed();
                }
                Destroy(gameObject);
            }
        }
        
    }
    // Gets the size of the other mote and calls in Absorb() when two motes collide
    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("absorbable")) {
            MoteScript otherMote = collision.gameObject.GetComponent<MoteScript>();
            Absorb(otherMote);
        }
    }
    void Color() { // Compare the moteSize of this gameobject to that of the player's mote
        rend = GetComponent<Renderer>(); // get the renderer component for Color()
        if (playerMote != null) {
            MoteScript playerMoteScript = playerMote.GetComponent<MoteScript>(); // Get the MoteScript component from the player's mote gameobject
            playerMoteSize = playerMoteScript.moteSize; // Get the size of the player's mote
            
            // Compare the size of the mote and the player, then change colors accordingly
            if (moteSize >= playerMoteSize) {
                rend.material.Lerp(bluemat, redmat, 100f);
                // rend.material = redmat;
                biggerThanPlayer = true;
            } else if (moteSize < playerMoteSize) {
                rend.material.Lerp(redmat, bluemat, 100f);
                // rend.material = bluemat;
                biggerThanPlayer = false;
            }
            // note that using Lerp causes everything to break and die, so don't un-comment those unless you wanna try and fix them somehow 
        } else {
            rend.material = redmat;
        }
    }
    void TimeScaler() { // if the mouse button is pressed down then tell the function to change the time scale as if the player were dragging a slider up or down
        if (allowTimeScaler) {
            if (Input.GetMouseButtonDown(1)) { // checks to see if the mouse button is down
                dragging = true; // set the bool to true
                prevMousePosY = Input.mousePosition.y; // set the variable for the y coordinate of the mouse in the previous frame
            } else if (Input.GetMouseButtonUp(1)) {dragging = false;} // if the mouse button is released then its no longer dragging the time scale up or down
            if (dragging){
                float currentMousePositionY = Input.mousePosition.y; // set the y position variable to the current y position of the mouse
                if (currentMousePositionY > prevMousePosY) { // if the current y position is greater than the one from the previous frame
                    if (Time.timeScale < 20f) { // clamp the time scale at a maximum of 10
                        Time.timeScale += 0.1f; // increase the time scale by .1 every frame
                    } else {Time.timeScale = 20f;} // if the time scale is greater than 10, set it back to 10 so that it can't be too high
                } else if (currentMousePositionY < prevMousePosY) { // if the current y position is less than the one from the previous frame
                    if (Time.timeScale > 0.5f) { // clamp the time scale at a minimum of 0.5
                        Time.timeScale -= 0.1f; // decrease the time scale by .1 every frame
                    } else {Time.timeScale = 0.5f;} // if the time scale is less than 0.5 then set it back to 0.5 so that it can't get too small
                }
                prevMousePosY = currentMousePositionY; // set the previous mouse position to the current mouse position so that the function will function properly next frame
                
                /*
                reference for Time.timeScale : https://docs.unity3d.com/ScriptReference/Time-timeScale.html
                time refers to the time at the beginnning of the frame and timeScale refers to how fast time passes, so when you put those two together you get the scale for how fast time passes in the scene
                so a Time.timeScale of 1 means the game is moving at its original speed while a Time.timeScale of 1.5 means that time is moving 1.5 times faster than usual
                */
            }
        }
    }
    void CinemachineZoom() {
        if (!Paused && allowZoom) {
            Vector2 scrollDelta = Input.mouseScrollDelta; // make it a variable for ease of access
            if (scrollDelta.y != 0) { // its you're scrolling
                targetSize -= sizeChangeAmount * scrollDelta.y; // change the size variable based on the scroll delta vector
                targetSize = Mathf.Clamp(targetSize, 0.5f, 45f); // Clamp the target size to a reasonable range
            }
            virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(virtualCamera.m_Lens.OrthographicSize, targetSize, zoomSpeed * Time.deltaTime); // finally lerp the size of the virtual camera (the one controlling the main camera) based on the target size and the zoom speed
            // this was a very difficult solution to find, but it was crucial that the camera wasn't super jumpy, so here's where i found how to change the Cinemachine's virtual camera size: https://docs.unity3d.com/Packages/com.unity.cinemachine@2.3/api/Cinemachine.LensSettings.html#:~:text=System.Single-,OrthographicSize,-When%20using%20an. it really would have been nice to know that is was so simple to begin with.
        }
    }
}