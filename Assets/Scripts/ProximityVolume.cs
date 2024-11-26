using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class ProximityVolume : MonoBehaviour
{
    [SerializeField] private AudioMixer proxMixer;

    public void SetVolume(float sliderValue){
        proxMixer.SetFloat("ProxVol", MathF.Log10(sliderValue) * 20);
    }
}
