using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] audioClips;  // Placeholder for menu option sounds
    [SerializeField] private AudioClip menuIntroClip; // Audio clip for blind player menu introduction
    [SerializeField] private Button[] menuButtons;    // Assign your buttons in the inspector

    private int selectedIndex = 0;   // To track which menu item is currently selected
    private bool hasNavigated = false;  // To prevent sound on initial selection

    void Start()
    {
        // Set the initial button selection (e.g., Start button)
        SelectButton(menuButtons[selectedIndex]);

        // Play the menu introduction for the blind player when the menu is first opened
        PlayMenuIntro();
    }

    void Update()
    {
        // Navigate down (next menu option)
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            NavigateMenu(1);  // Move to the next option
        }

        // Navigate up (previous menu option)
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            NavigateMenu(-1);  // Move to the previous option
        }

        // Trigger the currently selected button's action with Enter or Space
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            menuButtons[selectedIndex].onClick.Invoke();  // Invoke the button's onClick event
        }
    }

    // Method to handle menu navigation with keyboard input
    void NavigateMenu(int direction)
    {
        // Deselect the currently selected button
        menuButtons[selectedIndex].OnDeselect(null);

        // Calculate the new selected index (looping between options)
        selectedIndex = (selectedIndex + direction + menuButtons.Length) % menuButtons.Length;

        // Select the new button
        SelectButton(menuButtons[selectedIndex]);

        // Play the corresponding voice clip if navigation has occurred
        if (hasNavigated || selectedIndex != 0)
        {
            PlayOptionVoice(selectedIndex);
        }

        hasNavigated = true;  // Prevent sound on the initial selection
    }

    // Method to visually select a button and ensure it's highlighted
    void SelectButton(Button button)
    {
        button.Select();
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
}
