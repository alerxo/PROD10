using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [SerializeField] AudioSource AudioSource;
    [SerializeField] AudioClip [] audioClips;

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Funkar");
        }
    }
    public void GameLibrary()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Funkar");
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    
}
