using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterOnboarding : MonoBehaviour
{
    public AudioSource OnboardingAudio;
    public static bool isPlaying = false;

    public Monster monster;

    public void OnTriggerEnter(Collider other)
    {
        OnboardingAudio.Play();
        isPlaying = true;
    }

    private void Update()
    {
        if (isPlaying && !OnboardingAudio.isPlaying)
        {
            monster.gameObject.SetActive(true);
            isPlaying = false;
            Destroy(gameObject);
        }
    }
}
