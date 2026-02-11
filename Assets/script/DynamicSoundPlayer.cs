using UnityEngine;

public class DynamicSoundPlayer : MonoBehaviour
{
    // Assign the AudioSource component in the Inspector
    public AudioSource audioSource;

    // Assign your .wav file (AudioClip) in the Inspector
    public AudioClip mySoundClip;

    // Call this function to play the sound
    public void PlaySpecificSound()
    {
        // Ensure the source and clip are assigned
        if (audioSource != null && mySoundClip != null)
        {
            // Assign the clip to the source and play it
            audioSource.clip = mySoundClip;
            audioSource.Play();
        }
    }
}
