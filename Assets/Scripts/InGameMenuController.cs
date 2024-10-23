using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InGameMenuController : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] audioClips;  // Placeholder for menu option sounds
    [SerializeField] private AudioClip menuIntroClip; // Audio clip for blind player menu introduction
    [SerializeField] private Button[] menuButtons;    // Assign your buttons in the inspector
    [SerializeField] private GameObject inGameMenuController; // GameObject for menu control

    private int selectedIndex = 0;   // To track which menu item is currently selected
    private bool hasNavigated = false;  // To track if player has navigated
    private bool isPaused = false;  // To track if the game is paused
    private PlayerController playerController; // referens till Playercontroller

    void Start()
    {
        // Play the menu introduction for the blind player when the menu is first opened
        //PlayMenuIntro();
        inGameMenuController.GetComponentInChildren<Canvas>().enabled = false;

        // Do not automatically select the first button yet, wait for navigation
        hasNavigated = false;
    }

    void Update()
    {
        // Toggle pause state with Escape key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Escape pressed");
            if (isPaused == false)
            {
                PauseGame();
            }
            else
            {

                ResumeGame();
            }

        }

        if (isPaused)
        {
            // Navigate down (next menu option)
            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            {
                if (!hasNavigated)
                {
                    // On first navigation, just set the first option as the active one
                    ActivateFirstOption();
                }
                else
                {
                    NavigateMenu(1);  // Move to the next option
                }
            }

            // Navigate up (previous menu option)
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            {
                if (!hasNavigated)
                {
                    // On first navigation, just set the first option as the active one
                    ActivateFirstOption();
                }
                else
                {
                    NavigateMenu(-1);  // Move to the previous option
                }
            }

            // Trigger the currently selected button's action with Enter or Space
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            {
                if (hasNavigated)
                {
                    menuButtons[selectedIndex].onClick.Invoke();  // Invoke the button's onClick event
                }
            }
        }

    }

    // Pause the game and show the menu
    public void PauseGame()
    {
        inGameMenuController.GetComponentInChildren<Canvas>().enabled = true;  // ta fram menyn
        Time.timeScale = 0f;  // Pausa spel
        isPaused = true;
        PausAudio();
        Debug.Log("Audio should be paused");

        if (playerController != null)
        {
            playerController.SetPauseState(true); // pausa spelar kontroller
        }
    }

    // Resume the game and hide the menu
    public void ResumeGame()
    {
        inGameMenuController.GetComponentInChildren<Canvas>().enabled = false;  // Disable the menu
        Time.timeScale = 1f;  // Resume the game
        isPaused = false;
        ResumeAllAudioSources();
    }

    // Activate the first option when navigation begins
    void ActivateFirstOption()
    {
        selectedIndex = 0;
        PlayOptionVoice(selectedIndex);
        hasNavigated = true;
    }

    // Method to handle menu navigation with keyboard input
    void NavigateMenu(int direction)
    {
        // Calculate the new selected index (looping between options)
        selectedIndex = (selectedIndex + direction + menuButtons.Length) % menuButtons.Length;

        // Play the corresponding voice clip for the new selection
        PlayOptionVoice(selectedIndex);
    }

    // Play a voice clip corresponding to the selected menu option
    void PlayOptionVoice(int optionIndex)
    {
        audioSource.clip = audioClips[optionIndex];
        audioSource.Play();
    }

    // Play the menu introduction for blind players
    void PlayMenuIntro()
    {
        // This plays an audio clip introducing the player to the menu (e.g., "You are in the main menu.")
        audioSource.clip = menuIntroClip;  // Assign your introduction audio clip here
        audioSource.Play();
    }

    // Button actions
    public void StartGame()
    {
        SceneManager.LoadScene(1);  // Loads the first scene
    }

    public void GameLibrary()
    {
        // Handle opening the game library
    }

    public void QuitGame()
    {
        Application.Quit();  // Exits the game
    }

    void PausAudio()
    {
        
            AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();
            foreach (var audio in allAudioSources)
            {
                audio.Pause(); 
            }

        if (inGameMenuController.GetComponentInParent<AudioSource>())
        {
            inGameMenuController.GetComponentInParent<AudioSource>().Play();
        }

    }

    private void ResumeAllAudioSources()
    {
        AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();
        foreach (var audio in allAudioSources)
        {
            audio.UnPause(); 
        }
    }
}
