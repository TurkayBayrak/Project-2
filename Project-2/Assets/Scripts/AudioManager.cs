using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    
    private AudioSource audioSource;
    [SerializeField] private AudioClip[] clips;

    [SerializeField] private int[] notes;

    private void Awake()
    {
        if (!instance)
            instance = this;

        audioSource = GetComponent<AudioSource>();
    }

    public void PlayNote(int index)
    {
        audioSource.clip = clips[0];
        audioSource.pitch = Mathf.Pow(2, notes[index] / 12f);
        audioSource.Play();
    }

    public void PlayCutEffect()
    {
        audioSource.clip = clips[1];
        audioSource.pitch = 1;
        audioSource.Play();
    }
}
