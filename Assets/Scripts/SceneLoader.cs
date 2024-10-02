using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneLoader : MonoBehaviour
{
    // Start is called before the first frame update
   public void LoadAudioLibrary() 
    {
        SceneManager.LoadScene("AudioLibraryScene");
    }

   public void LoadStartMeny()
    {
        SceneManager.LoadScene("StartMenu");
    }
}
