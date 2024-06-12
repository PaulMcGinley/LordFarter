using UnityEngine;

public class BackgroundManager : MonoBehaviour {

    [SerializeField]
    GameObject player;

    Camera camera;

    [SerializeField]
    private GameObject[] backgrounds = new GameObject[3];
    float bg1Size => backgrounds[0].GetComponent<SpriteRenderer>().bounds.size.y;
    float bg1Scale => (5) + ((playerRiseSpeed - playerFallSpeed)*10);
    float bg2Size => backgrounds[1].GetComponent<SpriteRenderer>().bounds.size.y;
    float bg2ScaleRise => (5) + (playerRiseSpeed*15);
    float bg2ScaleFall => (5) + (playerFallSpeed*30);

    // Get Player riseSpeed from player attached script
    float playerRiseSpeed => player.GetComponent<Player>().riseSpeed;
    float playerFallSpeed => player.GetComponent<Player>().fallSpeed;
    bool isPlayerRising => player.GetComponent<Player>().isRising;

    // Start is called before the first frame update
    void Start() {

        camera = Camera.main;
    }

    // Update is called once per frame
    void Update() {

        backgrounds[0].transform.position = new Vector3(player.transform.position.x, camera.transform.position.y, 0);
        backgrounds[1].transform.position = new Vector3(player.transform.position.x - (player.transform.position.x/1000), camera.transform.position.y - (camera.transform.position.y/1000), 0);
        backgrounds[2].transform.position = new Vector3(player.transform.position.x - (player.transform.position.x/100), camera.transform.position.y - (camera.transform.position.y/100), 0);
    }
}
