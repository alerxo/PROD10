using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AudioLibraryController : MonoBehaviour
{
    public AudioSource audioSource;             // For playing the audio clips
    public AudioClip[] audioClips;              // Array of audio clips in the game
    public AudioClip clickSound;                // Sound for feedback when navigating the list
    public AudioClip introClip;                 // The introduction audio for the Audio Library scene
    public Text[] audioClipTexts;               // UI Text elements representing each audio clip

    private int selectedIndex = 0;              // To track which clip is selected

    void Start()
    {
        // Play the introduction voice when the scene starts
        PlayIntroVoice();

        // Highlight the first option
        SelectAudioClip(selectedIndex);
    }

    void Update()
    {
        // Navigate down (next clip)
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            NavigateList(1);  // Move to the next option
        }

        // Navigate up (previous clip)
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            NavigateList(-1);  // Move to the previous option
        }

        // Play the selected audio clip with Enter
        if (Input.GetKeyDown(KeyCode.Return))
        {
            PlaySelectedAudioClip();
        }
    }

    // Play the introductory audio when entering the scene
    void PlayIntroVoice()
    {
        audioSource.clip = introClip;
        audioSource.Play();
    }

    // Navigate the list with sound feedback
    void NavigateList(int direction)
    {
        // Deselect the current clip
        DeselectAudioClip(selectedIndex);

        // Calculate the new selected index (clamped to stay within bounds)
        selectedIndex = Mathf.Clamp(selectedIndex + direction, 0, audioClips.Length - 1);

        // Select the new clip and play the click sound
        SelectAudioClip(selectedIndex);
        audioSource.PlayOneShot(clickSound);  // Play clicking sound for feedback
    }

    // Highlight the selected audio clip in the UI
    void SelectAudioClip(int index)
    {
        audioClipTexts[index].color = Color.yellow;  // Change the text color to indicate selection
    }

    // Reset the visual state of the previously selected audio clip
    void DeselectAudioClip(int index)
    {
        audioClipTexts[index].color = Color.white;  // Change the text color back to default
    }

    // Play the selected audio clip
    void PlaySelectedAudioClip()
    {
        audioSource.clip = audioClips[selectedIndex];
        audioSource.Play();
    }
}
