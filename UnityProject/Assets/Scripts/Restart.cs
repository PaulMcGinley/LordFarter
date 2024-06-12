using UnityEngine;

public class Restart : MonoBehaviour {

    private Player playerScript;                                                                            // The player script
    private SpawnDoodads spawnDoodadsScript;                                                                // The doodads script
    private SpawnBeans spawnBeansScript;                                                                    // The beans script

    // Start is called before the first frame update
    void Start() {

        // Get the objects that need to be restarted
        playerScript = GameObject.Find("Player").GetComponent<Player>();                                    // Get the player script
        spawnDoodadsScript = GameObject.Find("Level Generation").GetComponent<SpawnDoodads>();              // Get the doodads script
        spawnBeansScript = GameObject.Find("Level Generation").GetComponent<SpawnBeans>();                  // Get the doodads script

        gameObject.transform.position = new Vector3(-10000, -1000, 0);                                      // Move the button to the middle of the screen
    }

    void Update() {

        if (playerScript.GetComponent<Player>().gas <= 0 && playerScript.GetComponent<Player>().onGround)  // If the player runs out of gas
            gameObject.transform.position = new Vector3(0, 0, 0);                                           // Move the button to the middle of the screen
    }

    public void OnClick(){

        // Restart the game
        playerScript.GetComponent<Player>().Restart();                                                      // Restart the player
        spawnDoodadsScript.GetComponent<SpawnDoodads>().Restart();                                          // Restart the doodads
        spawnBeansScript.GetComponent<SpawnBeans>().Restart();                                              // Restart the beans

        // Hide the restart button
        gameObject.transform.position = new Vector3(-10000, -1000, 0);                                      // Move the button to the middle of the screen
        gameObject.GetComponent<UnityEngine.UI.Button>().interactable = false;                              // Disable the button
    }
}
