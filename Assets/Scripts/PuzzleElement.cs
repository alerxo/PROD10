using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleElement : MonoBehaviour
{
    public AudioClip audioClip;
    public AudioClip solutionClip;
    public AudioClip responseClip;

    public bool solved = false;
    public bool solutionPlayed = false;

    void Start(){
        audioClip = gameObject.GetComponent<AudioSource>().clip;
    }

    public void Reset(){
        gameObject.GetComponent<AudioSource>().clip = audioClip;
        gameObject.GetComponent<AudioSource>().loop = true;
        gameObject.GetComponent<AudioSource>().Play();
        gameObject.GetComponent<Collider>().enabled = true;
    }
}
