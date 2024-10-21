using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneLoader : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject introOnboarding;
   public void LoadAudioLibrary() 
    {
        SceneManager.LoadScene("LearnGameAudioScene");
    }

   public void LoadStartMeny()
    {
        SceneManager.LoadScene("StartMenu");
    }

    public void LoadControlsMeny()
    {
        SceneManager.LoadScene("ControlsMenu");
    }

    public void StartGame()
    {
        SceneManager.LoadScene("IntroScene");
    }

    private void Update()
    {
        if (!introOnboarding.GetComponent<AudioSource>().isPlaying)
        {
            Debug.Log("Changing Scene");
            SceneManager.LoadScene("AdinPrototype");
        }
    }
    
}
