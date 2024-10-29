using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class PlayerVolumeScript : MonoBehaviour
{
    [SerializeField] private AudioMixer playerMixer;

    public void SetVolume(float sliderValue){
        playerMixer.SetFloat("PlayerVolume", MathF.Log10(sliderValue) * 20);
    }
}
