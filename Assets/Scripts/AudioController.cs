using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    // Start is called before the first frame update
    private AudioSource audioSource;
    private GameObject pc;
    Vector3 playerPos = new Vector3(0, 0, 0);
    public float maxDist = 15f;
    public float minDist = 5f;
    private float distance = 0f;
    void Start()
    {
        pc = GameObject.FindGameObjectWithTag("Player");
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        playerPos = pc.transform.position;
        distance = Vector3.Distance(playerPos, transform.position);
        AdjustVolume();
    }


    void AdjustVolume() {
        if(distance > maxDist) {
            audioSource.volume = 0f;
        } else if (distance < minDist){
            audioSource.volume = 1f;
        } else {
            audioSource.volume = distance;
        }
    }

    void PlaySound() {
        
    }
}
