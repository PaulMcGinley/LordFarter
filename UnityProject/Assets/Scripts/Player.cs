using System.Collections.Generic;
using System.Linq;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class Player : MonoBehaviour {

    bool endOfGame = false;

    #region Player

    // Sprite

    [SerializeField]
    GameObject sprite;                                                                                      // The player sprite graphic (idle sprite)
    float SpriteZ => sprite.transform.position.z;                                                           // The Z position of the sprite

    [SerializeField]
    GameObject goSprite;                                                                                    // Henry is going up sprite

    [SerializeField]
    GameObject byeSprite;                                                                                   // Henry is falling sprite

    // Augment values
    public float spriteXPos = 0;                                                                            // The new X position of the sprite

    Vector3 NewSpritePosition => new(spriteXPos, CameraY-3, SpriteZ);                                       // Lambda expression to return the new sprite position

    // Collision
    BoxCollider2D spriteCollider;                                                                           // The collider of the sprite

    // Game
    float score = 0;                                                                                        // The player's score
    public float gas = 1000;                                                                                       // Fart power

    #endregion

    #region HighScore Marker

    float highScore = 100;                                                                                  // The high score
    float lastHighScore = 100;                                                                                // The last high score

    [SerializeField]
    GameObject highScoreMarker;                                                                             // The high score marker (sprite graphic)

    #endregion

    #region Camera

    new Camera camera;                                                                                      // Game camera
    float CameraX => camera.transform.position.x;                                                           // The X position of the camera
    float CameraY => camera.transform.position.y;                                                           // The Y position of the camera
    float CameraZ => camera.transform.position.z;                                                           // The Z position of the camera

    // Augment values
    float cameraXPos = 0;                                                                                   // The new X position of the camera
    float cameraYPos = 0;                                                                                   // The new Y position of the camera

    Vector3 NewCameraPosition => new(cameraXPos, cameraYPos, CameraZ);                                      // Lambda expression to return the new camera position

    #endregion

    #region Movement

    public bool isRising = false;                                                                           // Flag to check if the camera is rising
    bool isFalling => !isRising && !onGround && fallSpeed > riseSpeed;                                      // Flag to check if the camera is falling
    const float riseRate = 0.005f;                                                                          // The rate at which the camera rises
    public float riseSpeed = 0f;                                                                            // The accumulated speed at which the camera rises
    const float riseSpeedMax = 0.5f;                                                                        // The maximum speed at which the camera can rise

    public bool onGround = true;                                                                                  // Flag to check if the player is on the ground

    const float fallRate = 0.005f;                                                                          // The rate at which the camera falls
    public float fallSpeed = 0f;                                                                            // The accumulated speed at which the camera falls
    const float fallSpeedMax = 0.5f;                                                                        // The maximum speed at which the camera can fall

    const float sideMoveSpeed = 0.1f;                                                                       // The speed at which the player moves left or right

    Touch touch;                                                                                            // The touch input

    Vector2 startTouchPosition;                                                                             // Position on screen where the touch started
    Vector2 currentTouchPosition;                                                                           // Position on screen where the touch is currently

    const float touchPadding = 50;                                                                          // Space to move the touch before it is considered a swipe
    bool SwipingLeft => currentTouchPosition.x < startTouchPosition.x - touchPadding;                       // Check if the player is swiping left
    bool SwipingRight => currentTouchPosition.x > startTouchPosition.x + touchPadding;                      // Check if the player is swiping right

    // Array of valid inputs for each movement
    readonly KeyCode[] riseInputs = { KeyCode.Space, KeyCode.W, KeyCode.UpArrow};                           // Array of inputs that make the player rise
    readonly KeyCode[] moveLeftInputs = { KeyCode.A, KeyCode.LeftArrow };                                   // Array of inputs that move the player left
    readonly KeyCode[] moveRightInputs = { KeyCode.D, KeyCode.RightArrow };                                 // Array of inputs that move the player right

    /// <summary>
    /// Check if the player is rising
    /// </summary>
    /// <returns>True if the player is rising, otherwise false</returns>
    bool IsMovingUp() {

        // Keyboard input
        if (riseInputs.Any(input => Input.GetKey(input)))                                                   // Check if any of the rise inputs are pressed
            return true;                                                                                    // We are rising

        // Touch input
        if (Input.touchCount > 0)                                                                           // Check if there is a touch input
            return true;                                                                                    // We are rising

        // No input
        return false;                                                                                       // We are not rising
    }

    /// <summary>
    /// Check if the player is moving left
    /// </summary>
    /// <returns>True if the player is moving left, otherwise false</returns>
    bool IsMovingLeft() {

        // Keyboard input
        if (moveLeftInputs.Any(input => Input.GetKey(input)))                                               // Check if any of the move left inputs are pressed
            return true;                                                                                    // We are moving left

        // Touch input
        if (SwipingLeft)                                                                                    // Check if the player is swiping left
            return true;                                                                                    // We are moving left

        // No input
        return false;                                                                                       // We are not moving left
    }

    /// <summary>
    /// Check if the player is moving right
    /// </summary>
    /// <returns>True if the player is moving right, otherwise false</returns>
    bool IsMovingRight() {

        // Keyboard input
        if (moveRightInputs.Any(input => Input.GetKey(input)))                                              // Check if any of the move right inputs are pressed
            return true;                                                                                    // We are moving right

        // Touch input
        if (SwipingRight)                                                                                   // Check if the player is swiping right
            return true;                                                                                    // We are moving right

        // No input
        return false;                                                                                       // We are not moving right
    }

    #endregion

    #region Sounds

    float nextFartTime = 0;                                                                                 // The next time the player can fart

    [SerializeField]
    float fartRateMin = 0.1f;                                                                               // The rate at which the player can fart

    [SerializeField]
    float fartRateMax = 0.5f;                                                                               // The rate at which the player can fart

    AudioSource audioSource;                                                                                // Audio source for the player

    [SerializeField]
    AudioClip[] landingSounds;                                                                              // Jump sound effect

    [SerializeField]
    AudioClip[] fartTones;                                                                                  // Fart sound effects
    float audioSourceStartingPitch;                                                                         // The starting pitch of the audio source (default pitch)

    [SerializeField]
    AudioClip[] screamSounds;                                                                               // Scream sound effects

    [SerializeField]
    AudioClip inhaleSound;                                                                                  // Inhale sound effect

    Queue<AudioClip> screamQueue = new();                                                                   // Queue of scream sound effects

    #endregion

    // Start is called before the first frame update
    void Start() {

        // Limit framerate to 60fps
        Application.targetFrameRate = 60;                                                                   // Set the frame rate to 60 as I can't seem to get the Lerp to work properly and time is running out :P

        // Get the main camera
        camera = Camera.main;

        // Get the sprite collider
        spriteCollider = sprite.GetComponent<BoxCollider2D>();

        // Get the audio source
        audioSource = GetComponent<AudioSource>();

        // Get the fart tone starting pitch
        audioSourceStartingPitch = audioSource.pitch;

        for (int i = 0; i < 500; i++) {                                                                     // Add 500 scream sound effects to the queue

            screamQueue.Enqueue(screamSounds[Random.Range(0, screamSounds.Length - 1)]);                    // Add a random scream sound effect to the queue
            screamQueue.Enqueue(inhaleSound);                                                               // Add the inhale sound effect to the queue
        }
    }

    public void Restart() {

        lastHighScore = highScore;                                                                                   // Set the last high score

        // Reset the player
        score = 0;                                                                                          // Reset the score
        gas = 1000;                                                                                         // Reset the gas
        cameraYPos = 0;                                                                                     // Reset the camera Y position
        cameraXPos = 0;                                                                                     // Reset the camera X position
        spriteXPos = 0;                                                                                     // Reset the sprite X position
        riseSpeed = 0;                                                                                      // Reset the rise speed
        fallSpeed = 0;                                                                                      // Reset the fall speed
        isRising = false;                                                                                   // Reset the isRising flag
        onGround = true;                                                                                    // Reset the onGround flag
        startTouchPosition = Vector2.zero;                                                                  // Reset the start touch position
        currentTouchPosition = Vector2.zero;                                                                // Reset the current touch position
        audioSource.pitch = audioSourceStartingPitch;                                                       // Reset the pitch of the audio source
        screamQueue.Clear();                                                                                // Clear the scream queue

        for (int i = 0; i < 500; i++) {                                                                     // Add 500 scream sound effects to the queue

            screamQueue.Enqueue(screamSounds[Random.Range(0, screamSounds.Length - 1)]);                    // Add a random scream sound effect to the queue
            screamQueue.Enqueue(inhaleSound);                                                               // Add the inhale sound effect to the queue
        }

        endOfGame = false;
    }

    // Update is called once per frame
    void Update() {

        GetUserInput();                                                                                     // Get the user input and move accordingly

        MovePlayer();                                                                                       // Move the player according to the user input

        CheckHighScore();                                                                                   // Check the high score and update the marker

        CheckForCollisions();                                                                               // Check if the player has collided with any objects

        CheckForLanding();                                                                                  // Check if the player has landed on the ground or platform

        SwitchSprite();                                                                                     // Switch the sprite graphic

        if (isFalling)                                                                                      // Check if the player is rising or falling
            ProcessScreamQueue();                                                                           // Play the scream sound effects

        if (gas <= 0 && cameraYPos <= 0)
            ShowRestartButton();
    }

    private void ShowRestartButton() {

        if (endOfGame) return;

        GameObject.Find("Button").GetComponent<UnityEngine.UI.Button>().transform.position -= new Vector3(0, 10000, 0);                                           // Move the button to the middle of the screen
        GameObject.Find("Button").GetComponent<UnityEngine.UI.Button>().interactable = true;

        endOfGame = true;
    }

    private void ProcessScreamQueue() {

        if (screamQueue.Count == 0) return;                                                                 // Check if the scream queue is empty

        if (!audioSource.isPlaying)                                                                         // Check if the audio source is not playing
            audioSource.PlayOneShot(screamQueue.Dequeue());                                                 // Play the next sound effect in the queue
    }

    #region Update Functions


    /// <summary>
    /// Get the user input and move the player accordingly
    /// </summary>
    void GetUserInput() {

        if (endOfGame) return;

        // Touch input
        if (Input.touchCount == 1) {                                                                        // Check if there is a touch input (only one touch at a time for better control)

            if (Input.GetTouch(0).phase == TouchPhase.Began) {                                              // Check if the touch has just started

                startTouchPosition = Input.GetTouch(0).position;                                            // Set the start touch position
            }

            touch = Input.GetTouch(0);                                                                      // Get the touch input

            currentTouchPosition = touch.position;                                                          // Set the current touch position
        }
        else {                                                                                              // No touch input

            startTouchPosition = Vector2.zero;                                                              // Reset the start touch position
            currentTouchPosition = Vector2.zero;                                                            // Reset the current touch position

#if !UNITY_EDITOR // testing, this directive around the code should be removed, makes for funnier falling screams
            audioSource.pitch = audioSourceStartingPitch;                                                   // Reset the pitch of the audio source
#endif
        }

        // Check if player is moving left
        if (IsMovingLeft()) {                                                                               // Check if a move left input is triggered

            if (isRising)                                                                                   // Check if the player is rising
                gas -= 0.1f;                                                                                // Reduce the gas by a small amount (free if falling)

            spriteXPos += -sideMoveSpeed;                                                                   // Move the sprite left

            if (spriteXPos < cameraXPos)                                                                    // ...
                cameraXPos += -sideMoveSpeed;                                                               // Move the camera left
        }

        // Check if player is moving right
        if (IsMovingRight()) {                                                                              // Check if a move right input is triggered

            if (isRising)                                                                                   // Check if the player is rising
                gas -= 0.1f;                                                                                // Reduce the gas by a small amount (free if falling)

            spriteXPos += sideMoveSpeed;                                                                    // Move the sprite right

            if (spriteXPos > cameraXPos)                                                                    // ...
                cameraXPos += sideMoveSpeed;                                                                // Move the camera right
        }

        // Check if player is moving up
        isRising = false;                                                                                   // Assume we are not rising

        // Rising by player input
        if (IsMovingUp() && gas > 0) {                                                                      // Check if the rise input is triggered

            isRising = true;                                                                                // Set the isRising flag to true
            riseSpeed += riseRate;                                                                          // Increase the riseSpeed
            fallSpeed -= fallRate * 0.5f;                                                                   // Reduce the fallSpeed
            gas -= 1;                                                                                       // Reduce the gas

            if (audioSource.pitch < 3f)                                                                     // Check if the pitch is not too high
                audioSource.pitch += 0.001f;                                                                // Set the pitch of the audio source

            if (Time.time > nextFartTime) {                                                                 // Check if the player can fart

                nextFartTime = Time.time + Random.Range(fartRateMin / audioSource.pitch, fartRateMax / audioSource.pitch);                          // Set the next time the player can fart (use pitch to adjust the rate of farting)
                audioSource.PlayOneShot(fartTones[Random.Range(0, fartTones.Length - 1)]);                  // Play the fart sound effect
            }
        }

        if (gas < 0) gas = 0;                                                                               // Check if the gas is empty, we don't want negative gas

        // Using up remaining riseSpeed before falling
        if ((!IsMovingUp() || gas == 0) && riseSpeed > 0)                                                   // Use up remaining riseSpeed (momentum)
            riseSpeed -= riseRate * 0.5f;                                                                   // Reduce the riseSpeed at a slower rate

        // Falling
        if (!isRising && !onGround)                                                                         // Check if we are not rising and the camera is above the ground
            fallSpeed += fallRate / (riseSpeed > 0 ? 2 : 1);                                                // If we still have riseSpeed, reduce the fallSpeed for a smoother transition

        // Check if the player is on the ground
        if (riseSpeed > 0)                                                                                  // Check if the player is rising
            onGround = false;                                                                               // Set the onGround flag to false
    }


    /// <summary>
    /// Move the player according to the user input
    /// </summary>
    void MovePlayer() {

        // Limit the speed values
        riseSpeed = Mathf.Clamp(riseSpeed, 0, riseSpeedMax);                                                // Limit the rise speed
        fallSpeed = Mathf.Clamp(fallSpeed, 0, fallSpeedMax);                                                // Limit the fall speed

        // Limit the sprite X positions
        spriteXPos = Mathf.Clamp(spriteXPos, -4f, 4f);                                                      // Limit the X position of the camera

        // Apply the new positions to the camera
        cameraYPos += riseSpeed - fallSpeed;                                                                // Calculate the new Y position of the camera

        // Limit the camera X positions
        cameraXPos = Mathf.Clamp(cameraXPos, -2.5f, 2.5f);                                                  // Limit the X position of the camera
        cameraYPos = Mathf.Clamp(cameraYPos, 0, float.MaxValue);                                            // Limit the Y position of the camera

        // Set the new positions
        camera.transform.position = NewCameraPosition;                                                      // Set the new camera position

        sprite.transform.position = NewSpritePosition;                                                      // Set the new sprite position
        goSprite.transform.position = NewSpritePosition;                                                    // Set the new sprite position
        byeSprite.transform.position = NewSpritePosition;                                                   // Set the new sprite position
    }


    private void CheckHighScore() {

        if (cameraYPos > score)                                                                             // Check if the camera has moved up
            score = cameraYPos;                                                                             // Set the new score (height of the camera

        if (score > highScore)                                                                              // Check if the score is higher than the high score
            highScore = score;                                                                              // Set the new high score

        highScoreMarker.transform.position = new Vector3(highScoreMarker.transform.position.x, lastHighScore-1, highScoreMarker.transform.position.z); // Move the high score marker
    }

    /// <summary>
    /// Check if the player has collided with any objects
    /// </summary>
    void CheckForCollisions() {

        BoxCollider2D[] colliders = FindObjectsOfType<BoxCollider2D>();                                     // Get all the colliders in the scene

        foreach (BoxCollider2D collider in colliders)                                                       // Loop through all the colliders
            if (collider.bounds.Contains(sprite.transform.position)) {                                      // Check if the collider contains the sprite

                if (collider == spriteCollider) continue;                                                   // Skip if the collider is the sprite collider (we don't want to collide with ourselves

                if (collider.gameObject.CompareTag("BeanPickup")) {                                         // Check if the collider is a bean pickup

                    gas += 100;                                                                             // Increase the players gas

                    collider.gameObject.GetComponent<BeanPickup>().EatBean();                                // Eat the bean
                    //Destroy(collider.gameObject);                                                           // Remove the object
                }
            }
    }


    /// <summary>
    /// Check if the player has landed on the ground or platform
    /// </summary>
    void CheckForLanding() {

        if (isRising) return;                                                                               // Skip if the player is rising (no need to check for landing
        if (onGround) return;                                                                               // Skip if the player is already on the ground

        if (CameraY <= 0) {                                                                                 // Check if the camera is at the ground

            camera.transform.position = new Vector3(CameraX, 0, CameraZ);                                   // Set the camera to the ground
            isRising = false;                                                                               // Set the isRising flag to false
            fallSpeed = 0;                                                                                  // Reset the fallSpeed
            riseSpeed = 0;                                                                                  // Reset the riseSpeed
            currentTouchPosition = Vector2.zero;                                                            // Reset the current touch position
            startTouchPosition = Vector2.zero;                                                              // Reset the start touch position
            onGround = true;                                                                                // Set the onGround flag to true

            audioSource.PlayOneShot(landingSounds[Random.Range(0, landingSounds.Length - 1)]);              // Play the landing sound
        }
    }


    /// <summary>
    /// Switch the sprite graphic based on the player's movement
    /// </summary>
    private void SwitchSprite() {

        if (isRising) {

            goSprite.SetActive(true);
            sprite.SetActive(false);
            byeSprite.SetActive(false);
        }
        else if (isFalling) {

            goSprite.SetActive(false);
            sprite.SetActive(false);
            byeSprite.SetActive(true);
        }
        // If for rising or falling that means we are not moving
        else {

            goSprite.SetActive(false);
            sprite.SetActive(true);
            byeSprite.SetActive(false);
        }
    }


    #endregion

    /// <summary>
    /// Draw the debug information on the screen
    /// </summary>
    void OnGUI() {

#if UNITY_EDITOR // Preprocessor directive to check if the game is running in the Unity Editor
        // GUI.Label(new Rect(10, 10, 100, 20), "--- DEBUG RUNNING ---", new GUIStyle() { fontSize = 50, fontStyle = FontStyle.Bold, normal = new GUIStyleState() { textColor = Color.red }});

        // // fps
        // GUI.Label(new Rect(10, 50, 100, 20), "FPS: " + (1.0f / Time.deltaTime), new GUIStyle() { fontSize = 50 });

        // // camera position
        // GUI.Label(new Rect(10, 100, 100, 20), "Camera X: " + CameraX, new GUIStyle() { fontSize = 50 });
        // GUI.Label(new Rect(10, 150, 100, 20), "Camera Y: " + CameraY, new GUIStyle() { fontSize = 50 });
        // GUI.Label(new Rect(10, 200, 100, 20), "Camera Z: " + CameraZ, new GUIStyle() { fontSize = 50 });

        // // rise speed
        // GUI.Label(new Rect(10, 250, 100, 20), "Rise Speed: " + riseSpeed, new GUIStyle() { fontSize = 50 });

        // // fall speed
        // GUI.Label(new Rect(10, 300, 100, 20), "Fall Speed: " + fallSpeed, new GUIStyle() { fontSize = 50 });

        // // is rising
        // GUI.Label(new Rect(10, 350, 100, 20), "Is Rising: " + isRising, new GUIStyle() { fontSize = 50 , normal = new GUIStyleState() { textColor = isRising ? Color.green : Color.red }});
#endif

        string _score = $"Score:";
        string _highScore = $"High Score:";
        string _height = $"Height:";
        string _gas = $"Gas: {gas:0.00} mB";
        string _speed = $"Speed: {(riseSpeed-fallSpeed)*60:0.00} m/s";

        DrawOutlinedText(_score, new Vector2(50, 10));
        DrawOutlinedText($"{score:0.00} m", new Vector2(400, 10));

        DrawOutlinedText(_highScore, new Vector2(50, 60));
        DrawOutlinedText($"{highScore:0.00} m", new Vector2(400, 60));

        DrawOutlinedText(_height, new Vector2(50, 110));
        DrawOutlinedText($"{cameraYPos:0.00} m", new Vector2(400,110));

        DrawOutlinedText(_gas, new Vector2(50, Screen.height - 50));
        DrawOutlinedText(_speed, new Vector2(50, Screen.height - 100));
    }

    void DrawOutlinedText(string text, Vector2 position) {

        GUIStyle style = new GUIStyle() { fontSize = 50, fontStyle = FontStyle.Bold, normal = new GUIStyleState() { textColor = Color.black } };

        // Draw the outline
        // I do this by drawing the text 8 times around the original text with a slight offset xD
        // This obviously is not the best way to do it but it works for now!
        GUI.Label(new Rect(position.x - 1, position.y - 1, 100, 20), text, style);
        GUI.Label(new Rect(position.x + 1, position.y + 1, 100, 20), text, style);
        GUI.Label(new Rect(position.x - 1, position.y + 1, 100, 20), text, style);
        GUI.Label(new Rect(position.x + 1, position.y - 1, 100, 20), text, style);

        GUI.Label(new Rect(position.x + 2, position.y, 100, 20), text, style);
        GUI.Label(new Rect(position.x - 2, position.y, 100, 20), text, style);
        GUI.Label(new Rect(position.x, position.y + 2, 100, 20), text, style);
        GUI.Label(new Rect(position.x, position.y - 2, 100, 20), text, style);

        // Draw the text
        style.normal.textColor = Color.white;                                                               // Set the text color to white
        GUI.Label(new Rect(position.x, position.y, 100, 20), text, style);                                  // Draw the text again
    }
}
