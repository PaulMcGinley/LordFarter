using UnityEngine;

public class SpawnBeans : MonoBehaviour {

    [SerializeField]
    GameObject beanPrefab;                                                                                  // The bean prefab to spawn

    GameObject[] beans;                                                                                     // The array of beans to spawn
    const int beanCount = 1000;                                                                             // The number of beans to spawn
    const float xRange = 3.5f;                                                                              // The range of x position to spawn the beans (+/- xRange from the center of the screen)
    float lastY = 0;                                                                                        // The last y position of the bean spawned
    const float minYDistance = 20;                                                                          // The minimum distance between beans
    const float maxYDistance = 50;                                                                          // The maximum distance between beans

    float RandomX => Random.Range(-xRange, xRange);                                                         // Lambda function to randomize the x position of the bean
    float RandomY => Random.Range(lastY + minYDistance, lastY + maxYDistance);                              // Lambda function to randomize the y position of the bean

    // Start is called before the first frame update
    void Start() {

        Restart();
    }

    public void Restart() {

        beans = new GameObject[beanCount];                                                                  // Initialize the array of beans

        // Loop through the number of beans to spawn
        for (int i = 0; i < beanCount; i++)
        {

            // Generate spawn position
            float x = RandomX;                                                                              // Randomize the x position of the bean
            float y = RandomY;                                                                              // Randomize the y position of the bean
            lastY = y;                                                                                      // Update the last y position of the bean spawned

            beans[i] = Instantiate(beanPrefab, new Vector3(x, y, 0), Quaternion.identity);                  // Instantiate the bean prefab
        }

        beans[^1].transform.position = new Vector3(0, 2.5f, 0);                                             // Move the last bean to the center of the screen (for the player to start with)
    }
}
