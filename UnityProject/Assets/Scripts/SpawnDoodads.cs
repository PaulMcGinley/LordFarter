using UnityEngine;

public class SpawnDoodads : MonoBehaviour {

    [SerializeField]
    GameObject prefabSatellite;                                                                             // The satellite prefab to spawn
    GameObject[] satellites;                                                                                // The array of satellites to spawn
    GameObject[] satellitesShadow;                                                                          // Shadow will be drawn on top of the satellite to give it depth

    int satelliteCount = 10;                                                                                // The number of satellites to spawn
    float xRange = 5f;                                                                                      // The range of x position to spawn the satellite (+/- xRange from the center of the screen)
    float lastY = 200;                                                                                      // The last y position of the satellite spawned
    float minYDistance = 50;                                                                                // The minimum distance between satellites
    float maxYDistance = 100;                                                                               // The maximum distance between satellites

    float RandomX => Random.Range(-xRange, xRange);                                                         // Lambda function to randomize the x position of the satellite
    float RandomY => Random.Range(lastY + minYDistance, lastY + maxYDistance);                              // Lambda function to randomize the y position of the satellite

    // Start is called before the first frame update
    void Start() {

        Restart();                                                                                          // Restart the game
    }

    public void Restart() {

        // Destroy all satellites
        // foreach (GameObject satellite in satellites)
        //     Destroy(satellite);                                                                             // Destroy the satellite

        // Initialize the array of satellites
        satellites = new GameObject[satelliteCount];                                                        // Initialize the array of doodads
        satellitesShadow = new GameObject[satelliteCount];                                                  // Initialize the array of doodads shadows

        // Loop through the number of doodads to spawn
        for (int i = 0; i < satelliteCount; i++) {

            // Generate spawn position
            float x = RandomX;                                                                              // Randomize the x position of the doodad
            bool flip = x < 0;                                                                              // Flip the satellite if it is on the left side of the screen

            float y = RandomY;                                                                              // Randomize the y position of the doodad
            lastY = y;                                                                                      // Update the last y position of the doodad spawned

            float scale = (float)(satelliteCount - i) / 10;                                                 // Calculate the scale of the satellite so that it gets smaller as it goes back

            // Instantiate the satellite prefab
            satellites[i] = Instantiate(prefabSatellite, new Vector3(x, y, 0), Quaternion.identity);        // Instantiate the satellite prefab
            satellitesShadow[i] = Instantiate(prefabSatellite, new Vector3(x, y, 0), Quaternion.identity);  // Instantiate the satellite shadow prefab

            // Set the scale of the satellite
            satellites[i].transform.localScale = new Vector3(x: flip ? -scale : scale, y: scale, z: 0);     // Set the scale of the satellite
            satellitesShadow[i].transform.localScale = new Vector3(x: flip ? -scale : scale, y: scale, z:0);// Set the scale of the satellite shadow

            // Set the color of the satellite shadow
            satellitesShadow[i].transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 1 - scale);   // Set the color of the satellite shadow
        }
    }
}
