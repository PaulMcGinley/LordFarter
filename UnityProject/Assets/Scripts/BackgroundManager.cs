using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundManager : MonoBehaviour {

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

    [SerializeField]
    GameObject player;

    Camera camera;

    // Start is called before the first frame update
    void Start() {

        camera = Camera.main;
    }

    // Update is called once per frame
    void Update() {

        backgrounds[0].transform.position = new Vector3(camera.transform.position.x, camera.transform.position.y, 0);
        backgrounds[1].transform.position = new Vector3(camera.transform.position.x, camera.transform.position.y - (camera.transform.position.y/1000), 0);
        backgrounds[2].transform.position = new Vector3(camera.transform.position.x, camera.transform.position.y - (camera.transform.position.y/100), 0);
       // backgrounds[1].transform.localScale = new Vector3(1, bg1Scale, 1);

       //backgrounds[2].transform.localScale = new Vector3(5, isPlayerRising ? bg2ScaleRise : bg2ScaleFall, 1);
    }
}
