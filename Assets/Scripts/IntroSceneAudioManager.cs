using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroSceneAudioManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] introClips;

    private void Awake()
    {
        GetComponent<AudioSource>().clip = introClips[ParameterSystem.Get().ActiveIntroClip];
        GetComponent<AudioSource>().Play();
    }
}
