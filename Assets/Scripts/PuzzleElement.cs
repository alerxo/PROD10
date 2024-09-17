using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleElement : MonoBehaviour
{
    public AudioClip audioClip;
    public AudioClip solutionClip;

    public AudioClip responseClip;

    void Start(){
        audioClip = gameObject.GetComponent<AudioSource>().clip;
    }
}
