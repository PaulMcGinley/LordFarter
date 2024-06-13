using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BeanPickup : MonoBehaviour {

    [SerializeField]
    AudioClip[] eatBeanSounds;

    AudioSource audioSource;

    // Start is called before the first frame update
    void Start() {

        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update() {

    }

    public void EatBean() {

        // audioSource.PlayOneShot(eatBeanSounds[Random.Range(0, eatBeanSounds.Length-1)]);                    // Play a random eat bean sound

        AudioSource.PlayClipAtPoint(eatBeanSounds[Random.Range(0, eatBeanSounds.Length-1)], transform.position); // Play a random eat bean sound
        Destroy(gameObject);                                                                                // Destroy the bean
    }
}
