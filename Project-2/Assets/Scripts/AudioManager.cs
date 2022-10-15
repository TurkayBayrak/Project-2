using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    
    private AudioSource _audioSource;
    public AudioClip[] clips;

    public int[] notes;

    private void Awake()
    {
        if (!instance)
            instance = this;

        _audioSource = GetComponent<AudioSource>();
    }

    public void PlayNote(int index)
    {
        _audioSource.clip = clips[0];
        _audioSource.pitch = Mathf.Pow(2, notes[index] / 12f);
        _audioSource.Play();
    }

    public void PlayCutEffect()
    {
        _audioSource.clip = clips[1];
        _audioSource.pitch = 1;
        _audioSource.Play();
    }
}
