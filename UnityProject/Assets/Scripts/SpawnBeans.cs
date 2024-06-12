using UnityEngine;

public class SpawnBeans : MonoBehaviour {

    [SerializeField]
    GameObject beanPrefab;                                                                                  // The bean prefab to spawn

    GameObject[] beans;                                                                                     // The array of beans to spawn
    int beanCount = 1000;                                                                                    // The number of beans to spawn
    float xRange = 2.5f;                                                                                    // The range of x position to spawn the beans (+/- xRange from the center of the screen)
    float lasyY = 0;                                                                                        // The last y position of the bean spawned
    float minYDistance = 20;                                                                                // The minimum distance between beans
    float maxYDistance = 50;                                                                               // The maximum distance between beans

    // Start is called before the first frame update
    void Start() {

        beans = new GameObject[beanCount];                                                                  // Initialize the array of beans

        // Loop through the number of beans to spawn
        for (int i = 0; i < beanCount; i++) {

            // Generate spawn position
            float x = Random.Range(-xRange, xRange);                                                        // Randomize the x position of the bean
            float y = Random.Range(lasyY + minYDistance, lasyY + maxYDistance);                             // Randomize the y position of the bean
            lasyY = y;                                                                                      // Update the last y position of the bean spawned

            // Instantiate a bean at the position (x, y, 0)
            beans[i] = Instantiate(beanPrefab, new Vector3(x, y, 0), Quaternion.identity) as GameObject;    // Instantiate the bean prefab
        }
    }

    // Update is called once per frame
    void Update() {

    }
}
