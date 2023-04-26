using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private float Volume = 1;
    private void Update()
    {
        AudioSource[] sources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource source in sources)
        {
            if (source.volume != Volume)
            {

                source.volume = Volume;
            }
        }
    }

    public void SetVolume(float volume)
    {
        Volume = volume;
    }
}