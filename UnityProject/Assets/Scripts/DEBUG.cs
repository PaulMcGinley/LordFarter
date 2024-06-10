using UnityEngine;

public class DEBUG : MonoBehaviour {

    Camera camera;
    float cameraX => camera.transform.position.x;
    float cameraY => camera.transform.position.y;
    float cameraZ => camera.transform.position.z;

    // Augment values
    float cameraXPos = 0;
    float cameraYPos = 0;

    Vector3 newCameraPosition => new Vector3(cameraXPos, cameraYPos, cameraZ);                              // Easier to have this as a property

    bool isRising = false;                                                                                  // Flag to check if the camera is rising
    const float riseRate = 0.005f;                                                                          // The rate at which the camera rises
    float riseSpeed = 0f;                                                                                   // The accumulated speed at which the camera rises
    const float riseSpeedMax = 0.5f;                                                                        // The maximum speed at which the camera can rise

    const float fallRate = 0.005f;                                                                          // The rate at which the camera falls
    float fallSpeed = 0f;                                                                                   // The accumulated speed at which the camera falls
    const float fallSpeedMax = 0.5f;                                                                        // The maximum speed at which the camera can fall

    // Start is called before the first frame update
    void Start() {

        // Limit framerate to 60fps
        Application.targetFrameRate = 60;                                                                   // Set the frame rate to 60 as I can't seem to get the Lerp to work properly

        // Get the main camera
        camera = Camera.main;
    }

    // Update is called once per frame
    void Update() {

        GetUserInput();                                                                                     // Get the user input and move accordingly

        // Limit the speed values
        riseSpeed = Mathf.Clamp(riseSpeed, 0, riseSpeedMax);                                                // Limit the rise speed
        fallSpeed = Mathf.Clamp(fallSpeed, 0, fallSpeedMax);                                                // Limit the fall speed

        // Limit the camera positions
        cameraXPos = Mathf.Clamp(cameraXPos, -2.5f, 2.5f);                                                  // Limit the X position of the camera
        cameraYPos = Mathf.Clamp(cameraYPos, 0, 80);                                                        // Limit the Y position of the camera

        // Calculate the new camera Y position
        cameraYPos += riseSpeed - fallSpeed;

        // Set the new camera position
        camera.transform.position = newCameraPosition;

        CheckForLanding();
    }

    void GetUserInput() {

        if (Input.GetKey(KeyCode.A))                                                                        // Check if the A key is pressed
            cameraXPos += -0.1f;                                                                            // Move the camera left

        if (Input.GetKey(KeyCode.D))                                                                        // Check if the D key is pressed
            cameraXPos += 0.1f;                                                                             // Move the camera right

        isRising = false;                                                                                   // Assume we are not rising

        if (Input.GetKey(KeyCode.Space)) {                                                                  // Check if the space key is pressed

            isRising = true;                                                                                // Set the isRising flag to true
            riseSpeed += riseRate;                                                                          // Increase the riseSpeed
            fallSpeed -= fallRate * 0.5f;                                                                   // Reduce the fallSpeed
        }

        if (!Input.GetKey(KeyCode.Space) && riseSpeed > 0)                                                  // Use up remaining riseSpeed (momentum)
            riseSpeed -= riseRate * 1.5f;                                                                   // Reduce the riseSpeed at a faster rate

        // Check for falling
        if (!isRising && cameraY > 0)                                                                       // Check if we are not rising and the camera is above the ground
            fallSpeed += fallRate;                                                                          // Increase the fall speed
    }

    void CheckForLanding() {

        if (cameraY <= 0.001) {                                                                             // Check if the camera is at the ground

            camera.transform.position = new Vector3(cameraX, 0, cameraZ);                                   // Set the camera to the ground
            isRising = false;                                                                               // Set the isRising flag to false
            fallSpeed = 0;                                                                                  // Reset the fallSpeed
            riseSpeed = 0;                                                                                  // Reset the riseSpeed
        }
    }

    void OnGUI() {

        GUI.Label(new Rect(10, 10, 100, 20), "--- DEBUG RUNNING ---", new GUIStyle() { fontSize = 50, fontStyle = FontStyle.Bold, normal = new GUIStyleState() { textColor = Color.red }});

        // fps
        GUI.Label(new Rect(10, 50, 100, 20), "FPS: " + (1.0f / Time.deltaTime), new GUIStyle() { fontSize = 50 });

        // camera position
        GUI.Label(new Rect(10, 100, 100, 20), "Camera X: " + cameraX, new GUIStyle() { fontSize = 50 });
        GUI.Label(new Rect(10, 150, 100, 20), "Camera Y: " + cameraY, new GUIStyle() { fontSize = 50 });
        GUI.Label(new Rect(10, 200, 100, 20), "Camera Z: " + cameraZ, new GUIStyle() { fontSize = 50 });

        // rise speed
        GUI.Label(new Rect(10, 250, 100, 20), "Rise Speed: " + riseSpeed, new GUIStyle() { fontSize = 50 });

        // fall speed
        GUI.Label(new Rect(10, 300, 100, 20), "Fall Speed: " + fallSpeed, new GUIStyle() { fontSize = 50 });

        // is rising
        GUI.Label(new Rect(10, 350, 100, 20), "Is Rising: " + isRising, new GUIStyle() { fontSize = 50 , normal = new GUIStyleState() { textColor = isRising ? Color.green : Color.red }});

    }
}