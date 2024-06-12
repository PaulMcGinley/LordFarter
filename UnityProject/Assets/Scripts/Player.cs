using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour {

    #region Player

    // Sprite

    [SerializeField]
    GameObject sprite;                                                                                      // The player sprite graphic
    float SpriteZ => sprite.transform.position.z;                                                           // The Z position of the sprite

    // Augment values
    float spriteXPos = 0;                                                                                   // The new X position of the sprite

    Vector3 NewSpritePosition => new(spriteXPos, CameraY-3, SpriteZ);                                       // Lambda expression to return the new sprite position

    // Collision
    BoxCollider2D spriteCollider;                                                                           // The collider of the sprite

    // Game
    float score = 0;                                                                                        // The player's score
    float gas = 1000;                                                                                       // Fart power

    #endregion


    #region HighScore Marker

    float highScore = 100;                                                                                    // The high score
    [SerializeField]
    GameObject highScoreMarker;                                                                             // The high score marker

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

    public bool isRising = false;                                                                                  // Flag to check if the camera is rising
    const float riseRate = 0.005f;                                                                          // The rate at which the camera rises
    public float riseSpeed = 0f;                                                                                   // The accumulated speed at which the camera rises
    const float riseSpeedMax = 0.5f;                                                                        // The maximum speed at which the camera can rise

    bool onGround = false;                                                                                  // Flag to check if the player is on the ground

    const float fallRate = 0.005f;                                                                          // The rate at which the camera falls
    public float fallSpeed = 0f;                                                                                   // The accumulated speed at which the camera falls
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


    // Start is called before the first frame update
    void Start() {

        // Limit framerate to 60fps
        Application.targetFrameRate = 60;                                                                   // Set the frame rate to 60 as I can't seem to get the Lerp to work properly and time is running out :P

        // Get the main camera
        camera = Camera.main;

        // Get the sprite collider
        spriteCollider = sprite.GetComponent<BoxCollider2D>();
    }


    // Update is called once per frame
    void Update() {

        GetUserInput();                                                                                     // Get the user input and move accordingly

        MovePlayer();                                                                                       // Move the player according to the user input

        // Move box collider with sprite
       // spriteCollider.transform.position = sprite.transform.position;                                       // Move the collider with the sprite

        if (cameraYPos > score)                                                                             // Check if the camera has moved up
            score = cameraYPos;                                                                             // Set the new score (height of the camera

        if (score > highScore)                                                                              // Check if the score is higher than the high score
            highScore = score;                                                                              // Set the new high score

        highScoreMarker.transform.position = new Vector3(highScoreMarker.transform.position.x, highScore-1, highScoreMarker.transform.position.z); // Move the high score marker

        // Get all BoxCollider2d objects in the scene
        BoxCollider2D[] colliders = FindObjectsOfType<BoxCollider2D>();

        // Check if the player has collided with any of the colliders
        foreach (BoxCollider2D collider in colliders) {

            if (collider.bounds.Contains(sprite.transform.position)) {                                       // Check if the collider contains the sprite

            if (collider == spriteCollider) continue;                                                      // Skip if the collider is the sprite collider (we don't want to collide with ourselves

                Debug.LogWarning("Collision detected");
                Debug.LogWarning(collider.gameObject.name);

                if (collider.gameObject.CompareTag("BeanPickup")) {

                    gas += 100;                                                                             // Increase the gas
                    // If you want to destroy the BeanPickup after the collision
                    Destroy(collider.gameObject);
                }
            }
        }


        CheckForLanding();                                                                                  // Check if the player has landed on the ground or platform
    }



    // void OnTriggerEnter2D(Collider2D other) {

    //     Debug.LogWarning("Collision detected");
    //     Debug.LogWarning(other.gameObject.name);

    //     if (other.gameObject.CompareTag("BeanPickup")) {

    //         gas += 100;                                                                                     // Increase the gas
    //         // If you want to destroy the BeanPickup after the collision
    //         Destroy(other.gameObject);
    //     }
    // }


    /// <summary>
    /// Get the user input and move the player accordingly
    /// </summary>
    void GetUserInput() {

        // Touch input
        if (Input.touchCount == 1) {                                                                        // Check if there is a touch input (only one touch at a time for better control)

            if (Input.GetTouch(0).phase == TouchPhase.Began)                                                // Check if the touch has just started
                startTouchPosition = Input.GetTouch(0).position;                                            // Set the start touch position

            touch = Input.GetTouch(0);                                                                      // Get the touch input

            currentTouchPosition = touch.position;                                                          // Set the current touch position
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
        }

        if (gas < 0) gas = 0;                                                                               // Check if the gas is empty, we don't want negative gas

        // Using up remaining riseSpeed before falling
        if ((!IsMovingUp() || gas == 0) && riseSpeed > 0)                                                                 // Use up remaining riseSpeed (momentum)
            riseSpeed -= riseRate * 1.5f;                                                                   // Reduce the riseSpeed at a faster rate

        // Falling
        if (!isRising && !onGround)                                                                       // Check if we are not rising and the camera is above the ground
            fallSpeed += fallRate;                                                                          // Increase the fall speed


        if (riseSpeed > 0)                                                                                 // Check if the player is rising
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
        cameraYPos = Mathf.Clamp(cameraYPos, 0, float.MaxValue);                                                        // Limit the Y position of the camera

        // Set the new positions
        camera.transform.position = NewCameraPosition;                                                      // Set the new camera position
        sprite.transform.position = NewSpritePosition;                                                      // Set the new sprite position
    }

    /// <summary>
    /// Check if the player has landed on the ground or platform
    /// </summary>
    void CheckForLanding() {

        if (isRising) return;                                                                              // Skip if the player is rising (no need to check for landing
        if (onGround) return;                                                                               // Skip if the player is already on the ground

        if (CameraY <= 0) {                                                                                 // Check if the camera is at the ground

            camera.transform.position = new Vector3(CameraX, 0, CameraZ);                                   // Set the camera to the ground
            isRising = false;                                                                               // Set the isRising flag to false
            fallSpeed = 0;                                                                                  // Reset the fallSpeed
            riseSpeed = 0;                                                                                  // Reset the riseSpeed
            currentTouchPosition = Vector2.zero;                                                            // Reset the current touch position
            startTouchPosition = Vector2.zero;                                                              // Reset the start touch position
            onGround = true;                                                                                // Set the onGround flag to true
        }
    }


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

        string _score = $"Score: {score.ToString("0.00")}m";
        string _height = $"Height: {cameraYPos:0.00m}";
        string _gas = $"Gas: {gas:0.00}";

        DrawOutlinedText(_score, new Vector2(10, 10));
        DrawOutlinedText(_height, new Vector2(10, 60));

        DrawOutlinedText(_gas, new Vector2(10, Screen.height - 50));
    }

    void DrawOutlinedText(string text, Vector2 position) {

        GUIStyle style = new GUIStyle() { fontSize = 50, fontStyle = FontStyle.Bold, normal = new GUIStyleState() { textColor = Color.black } };

        // Draw the outline
        GUI.Label(new Rect(position.x - 1, position.y - 1, 100, 20), text, style);
        GUI.Label(new Rect(position.x + 1, position.y + 1, 100, 20), text, style);
        GUI.Label(new Rect(position.x - 1, position.y + 1, 100, 20), text, style);
        GUI.Label(new Rect(position.x + 1, position.y - 1, 100, 20), text, style);

        GUI.Label(new Rect(position.x + 2, position.y, 100, 20), text, style);
        GUI.Label(new Rect(position.x - 2, position.y, 100, 20), text, style);
        GUI.Label(new Rect(position.x, position.y + 2, 100, 20), text, style);
        GUI.Label(new Rect(position.x, position.y - 2, 100, 20), text, style);

        // Draw the text
        style.normal.textColor = Color.white;
        GUI.Label(new Rect(position.x, position.y, 100, 20), text, style);
    }
}
