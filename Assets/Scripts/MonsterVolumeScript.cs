using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MonsterVolumeScript : MonoBehaviour
{
    [SerializeField] private AudioMixer monsterMixer;

    public void SetVolume(float sliderValue){
        monsterMixer.SetFloat("MonsterVolume", MathF.Log10(sliderValue) * 20);
    }
}
