using UnityEngine;

public class BackgroundManager : MonoBehaviour {

    [SerializeField]
    GameObject player;                                                                                      // The player object (used to get the player's position)

    Camera camera;                                                                                          // The main camera object

    [SerializeField]
    private GameObject[] backgrounds = new GameObject[3];                                                   // The array of backgrounds

    // Start is called before the first frame update
    void Start() {

        camera = Camera.main;
    }

    // Update is called once per frame
    void Update() {

        // Update the position of the backgrounds based on the player's position
        backgrounds[0].transform.position = new Vector3(player.transform.position.x, camera.transform.position.y, 0);
        backgrounds[1].transform.position = new Vector3(player.transform.position.x - (player.transform.position.x/1000), camera.transform.position.y - (camera.transform.position.y/1000), 0);
        backgrounds[2].transform.position = new Vector3(player.transform.position.x - (player.transform.position.x/100), camera.transform.position.y - (camera.transform.position.y/100), 0);
    }
}
