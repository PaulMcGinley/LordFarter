using UnityEngine;

public class SpawnDoodads : MonoBehaviour {

    [SerializeField]
    GameObject prefabSatellite;                                                                             // The satellite prefab to spawn
    GameObject[] satellites;                                                                                // The array of satellites to spawn
    GameObject[] satellitesShadow;                                                                          // Shadow will be drawn on top of the satellite to give it depth

    int satelliteCount = 10;                                                                                // The number of satellites to spawn
    float xRange = 5f;                                                                                      // The range of x position to spawn the satellite (+/- xRange from the center of the screen)
    float lasyY = 200;                                                                                      // The last y position of the satellite spawned
    float minYDistance = 50;                                                                                // The minimum distance between satellites
    float maxYDistance = 100;                                                                               // The maximum distance between satellites

    [SerializeField]
    GameObject prefabUFO;
    GameObject[] UFOs;

    int UFOCount = 10;
    float UFOxRange = 2.5f;
    float UFOyRange = 2.5f;
    float lastUFOY = 0;
    float minUFOYDistance = 20;
    float maxUFOYDistance = 50;


    // Start is called before the first frame update
    void Start() {

        satellites = new GameObject[satelliteCount];                                                        // Initialize the array of doodads
        satellitesShadow = new GameObject[satelliteCount];                                                  // Initialize the array of doodads


        // Loop through the number of doodads to spawn
        for (int i = 0; i < satelliteCount; i++) {

            // Generate spawn position
            float x = Random.Range(-xRange, xRange);                                                        // Randomize the x position of the doodad
            bool flip = x < 0;
            float y = Random.Range(lasyY + minYDistance, lasyY + maxYDistance);                             // Randomize the y position of the doodad
            lasyY = y;                                                                                      // Update the last y position of the doodad spawned
            float scale = ((float)(satelliteCount - i)/10);

            satellites[i] = Instantiate(prefabSatellite, new Vector3(x, y, 0), Quaternion.identity);        // Instantiate the satellite prefab
            satellitesShadow[i] = Instantiate(prefabSatellite, new Vector3(x, y, 0), Quaternion.identity);  // Instantiate the satellite shadow prefab
            satellites[i].transform.localScale = new Vector3((flip ? -scale : scale), scale, 0);            // Set the scale of the satellite
            satellitesShadow[i].transform.localScale = new Vector3((flip ? -scale : scale), scale, 0);      // Set the scale of the satellite shadow

            satellitesShadow[i].transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 1-scale);   // Set the color of the satellite shadow
        }
    }

    // Update is called once per frame
    void Update() {
    }
}
