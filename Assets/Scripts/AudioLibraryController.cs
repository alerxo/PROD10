using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class AudioLibraryController : MonoBehaviour
{
    public AudioSource audioSource;   // AudioSource for playing sounds
    public AudioClip introClip;       // Sound explaining the audio library on scene start
    public AudioClip[] audioClips;    // Array of audio clips in the game
    public AudioClip[] voiceClips;    // Array of voice clips linked to each button
    public AudioClip clickSound;      // Sound to play when scrolling between buttons
    public AudioClip endSound;        // Sound to play when trying to scroll beyond the list
    public Button[] audioClipButtons; // Buttons representing each audio clip
    public Button mainMenuButton;     // Button to return to the main menu
    private int selectedIndex = 0;    // Tracks which button is selected

    void Start()
    {
        // Play the introductory sound when entering the scene
        PlayIntroClip();

        // Assign buttons to play the corresponding audio clip
        for (int i = 0; i < audioClipButtons.Length; i++)
        {
            int index = i;  // Local copy of index for the lambda function
            audioClipButtons[i].onClick.AddListener(() => PlayAudioClip(index));
        }

        // Assign the Main Menu button to take the player back to the main page
        mainMenuButton.onClick.AddListener(GoToMainMenu);

        // Set the Main Menu button as the default selected button
        EventSystem.current.SetSelectedGameObject(audioClipButtons[0].gameObject);  // Start with the first audio clip button selected
    }

    void Update()
    {
        // Handle scrolling up
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            if (selectedIndex >= 0)
            {
                selectedIndex--;
                PlayClickSound();
                PlayVoiceClip(selectedIndex);
                EventSystem.current.SetSelectedGameObject(audioClipButtons[selectedIndex].gameObject);
            }
            else
            {
                PlayEndSound();  // Play end sound if trying to scroll up beyond the first button
            }
        }

        // Handle scrolling down
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            if (selectedIndex < audioClipButtons.Length - 1)
            {
                selectedIndex++;
                PlayClickSound();
                PlayVoiceClip(selectedIndex);
                EventSystem.current.SetSelectedGameObject(audioClipButtons[selectedIndex].gameObject);
            }
            else
            {
                PlayEndSound();  // Play end sound if trying to scroll down beyond the last button
            }
        }

        // Handle pressing Enter/Return
        if (Input.GetKeyDown(KeyCode.Return))
        {
            // Check if the Back Button (Main Menu Button) is selected
            if (EventSystem.current.currentSelectedGameObject == mainMenuButton.gameObject)
            {
                GoToMainMenu();  // Trigger the scene switch if Back Button is selected
            }
            else
            {
                PlayAudioClip(selectedIndex);  // Otherwise, play the selected audio clip
            }
        }
    }

    // Play the introduction sound when entering the scene
    void PlayIntroClip()
    {
        audioSource.clip = introClip;
        audioSource.Play();
    }

    // Play the audio clip associated with a selected button
    void PlayAudioClip(int clipIndex)
    {
        audioSource.clip = audioClips[clipIndex];
        audioSource.Play();
    }

    // Play the voice clip for the selected button
    void PlayVoiceClip(int index)
    {
        audioSource.PlayOneShot(voiceClips[index]);
    }

    // Play the clicking sound when scrolling through the buttons
    void PlayClickSound()
    {
        audioSource.PlayOneShot(clickSound);
    }

    // Play the end sound when trying to scroll past the list limits
    void PlayEndSound()
    {
        audioSource.PlayOneShot(endSound);
    }

    // Go back to the main menu when the player selects the Main Menu button
    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MenuControllerScene");  // Replace with the actual main menu scene name
    }
}
