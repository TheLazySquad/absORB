using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MoteScript : MonoBehaviour {
    // Universal mote variables
    public bool IsPlayer; // Check if the mote is a player mote
    public Rigidbody2D rb;
    Renderer rend;
    public static List<MoteScript> Motes;
    public float moteSize = 1.5f; // The size of the mote, used to calculate the actual size of the mote when the game starts
    public float GConstant = 0.05f; // The gravitational constant for the attraction between all motes

    // Variables for OutOfBounds()
    public float OOBDrag = 1f; // The value of the drag force applied by the rigidbody when the mote is outside the set boundaries defined below
    public float OOBForce = 0.1f; // The amount of force applied to the rigidbody when the mote is outside the set boundaries defined below
    private float xBoundary = 20; private float yBoundary = 20; // The coordinates used in OutOfBounds()
    
    // Player mote specific variables
    public GameObject motePrefab; // Reference to the mote prefab
    public float moteSpawnDistance = 3.0f; // The distance to spawn the cloned mote away from the player mote
    public float moteSpawnSize = 0.1f; // The size to set the cloned mote to
    public float moteSpawnForce = 0.1f; // The amount of force to apply to the cloned mote
    public float playerLaunchForce = 0.05f; // Originally I was using the moteSpawnForce to push the player mote away with the same force - like how equal and opposite reactions work in real life - but that didn't give me the effect I was looking for
    
    // Pause Menu and Time
    public GameObject PlayerTooSmallUI;
    public GameObject pauseMenu;
    public bool Paused = false;
    public float gameSpeedTime = 1f;

    // Variables for Color()
    public Material bluemat; // Blue material used to designate motes that the player can absorb
    public Material redmat; // Red material used to designate motes that the player cannot absorb
    public GameObject playerMote; // gets the player mote for Color()
    public float playerMoteSize; // the variable for the size of the player gameobject for Color()

    void OnEnable() {
        if (Motes == null)
            Motes = new List<MoteScript>(); // Create the list if it hasn't been created already.
        Motes.Add(this); // Add all motes to the list when this line is run on each mote
        
        rend = GetComponent<Renderer>(); // get the renderer component for Color()
    }

    void OnDisable() {
        Motes.Remove(this); // If the mote this script is attached to is disabled, then remove it from the list
    }
    public void ReloadScene() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // using GetActiveScene instead of just using the name of the scene is nice because i made all of the levels their own scene. https://docs.unity3d.com/ScriptReference/SceneManagement.SceneManager.GetActiveScene.        
    } 
    void FixedUpdate() {
        //Out Of Bounds
        OutOfBounds();

        //Gravity
        foreach (MoteScript attractor in Motes) {
            if (attractor != this) {
                Gravity(attractor);
            }
        }

    }
    void Update() {
        transform.localScale = new Vector3(moteSize * 0.01f, moteSize * 0.01f, 1f); // Calculates the size of the sprite based on the variable moteSize
        
        playerMote = GameObject.FindGameObjectWithTag("Player"); // Find the gameobject with the IsPlayer flag set to true

        // I put the player stuff under Update instead of Update because when it was under FixedUpdate, it would spawn a new mote every frame that the mouse was pressed down because its state gets refreshed every frame and thought it was getting pressed each frame instead of held from when it was initally pressed
        if(IsPlayer){
            Player();
        }

        // Change the color based on player size
        if (IsPlayer == false) {
            Color();
        }
    }
        
    void PlayerTooSmall() {
        Debug.Log("player is too small"); // Outputs the message to the console when the player is too small to shoot out more mass to move itself around. this was originaly just a placeholder, but I think it's kind of useful to see so I can tell if the mote spawner is working or not
        playerMote.SetActive(false);
        PlayerTooSmallUI.SetActive(true);
        Debug.Log("Ui enabled");
    }
    void Player() {
        if(moteSize > (1.5f * moteSpawnSize)) { // Makes sure that the player mote can't get too small
            if (Input.GetKeyDown(KeyCode.Escape)) { // Check if the escape key is pressed
                Debug.Log("ESC pressed"); // log statment
                if (Paused) {
                    MenuClosed(); // run code specific to when the menu is open
                }
                else if (!Paused) {
                    MenuOpened();
                }
            }
            if(!Paused){
                if (Input.GetMouseButtonDown(0)) { // Check if the left mouse button is pressed down --> https://docs.unity3d.com/ScriptReference/Input.GetMouseButtonDown.html
                    
                    // Rotates the player mote such that its x axis is always facing the mouse
                    Vector3 mousePos = Input.mousePosition;
                    mousePos.z = 10f;
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
        }
        else {
            PlayerTooSmall();
        }
    }
    void MenuOpened() {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f; // source: https://docs.unity3d.com/ScriptReference/Time-timeScale.html
        Paused = true;
        }
    public void MenuClosed() {
        pauseMenu.SetActive(false);
        Time.timeScale = gameSpeedTime;
        Paused = false;
    }
    void OutOfBounds() { // Defines what will happen when the mote is past a certain distance (xBoundary & yBoundary) in each direction.
        if (-xBoundary > rb.transform.position.x) {
            rb.AddForce(Vector2.right*OOBForce);
            rb.drag = OOBDrag;
        }
        else if (xBoundary < rb.transform.position.x) {
            rb.AddForce(Vector2.left*OOBForce);
            rb.drag = OOBDrag;
        }
        else if (-yBoundary > rb.transform.position.y) {
            rb.AddForce(Vector2.up*OOBForce);
            rb.drag = OOBDrag;
        }
        else if (yBoundary < rb.transform.position.y) {
            rb.AddForce(Vector2.down*OOBForce);
            rb.drag = OOBDrag;
        }
        else {
            rb.drag = 0.00f; // Universal drag constant. All motes will always feel this drag on their rigidbody while inside the defined boundaries. I don't want to set it to a variable because there is no reason it should ever not be 0 unless I want to change how the entire game feels
        }
    }
    // Calculate and apply a constant simulated gravitational force to all motes
    void Gravity(MoteScript affectedObject) {
        Rigidbody2D affectedRigidbody = affectedObject.rb;
        Vector2 direction = rb.position - affectedRigidbody.position;
        float distance = direction.magnitude;
        float forceStrength = GConstant * (rb.mass * affectedRigidbody.mass) / Mathf.Pow(distance, 2); // Gravitational Force = The Gravitational Constant x First Object's Mass x Second Object's Mass / Distance between the two objects. Thanks Isaac Newton!
        Vector2 force = direction.normalized * forceStrength; // Normalizing the direction & multiplying that by the strength of the force from the Gravitational Force equation ^  
        affectedRigidbody.AddForce(force); // Applying "gravity" force
    }

    // The Absorb function is part of what happens when two motes collide
    void Absorb(MoteScript otherMote) {
        // Compare the sizes of the two colliding motes
        if (moteSize > otherMote.moteSize) {
            // Absorb the other mote if this mote is bigger
            moteSize += otherMote.moteSize;
            Destroy(otherMote.gameObject);
        }
        else if (moteSize < otherMote.moteSize) {
            // Absorb this mote if the other mote is bigger
            otherMote.moteSize += moteSize;
            Destroy(gameObject);
        }
        else {
            /* 
            Currently this will do nothing if they are the same size
            I don't know what exactly I want them to do if they are the same size, but here are some options:
            Debug.Log("two motes with the same size have collided and I haven't decided what to do with that yet");
            if (otherMote.moteSize == moteSize) {Destroy(otherMote.gameObject); Destroy(gameObject);} // destroys them both
            if (otherMote.moteSize == moteSize) {otherMote.rb.AddForce(Random.insideUnitCircle.normalized * Random.Range(1f,5f));} // sends them in a random direction with a random force
            */
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
        if (playerMote != null) {
            MoteScript playerMoteScript = playerMote.GetComponent<MoteScript>(); // Get the MoteScript component from the player's mote gameobject
            playerMoteSize = playerMoteScript.moteSize; // Get the moteSize variable from the player's mote
            
            // Compare the size of the mote and the player, then change colors accordingly
            if (moteSize >= playerMoteSize)
            {
                // rend.material.Lerp(bluemat, redmat, Time.time);
                rend.material = redmat;
            }
            else if (moteSize < playerMoteSize)
            {
                // rend.material.Lerp(redmat, bluemat, Time.time);
                rend.material = bluemat;
            }    
        }
        else {
            rend.material = redmat;
        }
    }
}