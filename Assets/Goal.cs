using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Goal : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip victorySound;
    public AudioClip callSound;
    private bool goToMenu;

    private void Start() {
        audioSource = GetComponent<AudioSource>();
    }

    public void DaugherResponse() {
        audioSource.PlayOneShot(callSound);
    }
    
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            audioSource.PlayOneShot(victorySound);
            StartCoroutine(waitForFinish());
        }
    }

    private IEnumerator waitForFinish()
    {
        while (audioSource.isPlaying)
        {
            yield return null;
        }

        SceneManager.LoadScene("StartMenu");
    }
}
