using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class radio : MonoBehaviour
{
    private AudioSource aSource;
    public List<AudioClip> songs = new List<AudioClip>();
    private int currentSong;

    // Start is called before the first frame update
    void Start()
    {
        aSource = GetComponent<AudioSource>();
        shuffle(songs);
    }

    void shuffle(List<AudioClip> radio)
    {
        for (int t = 0; t < radio.Count; t++)
        {
            AudioClip tmp = radio[t];
            int r = Random.Range(t, radio.Count);
            radio[t] = radio[r];
            radio[r] = tmp;
        }
        currentSong = 0;
        aSource.clip = songs[0];
        aSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (!aSource.isPlaying)
        {
            if (currentSong+1 <= songs.Count-1)
            {
                currentSong++;
            }
            else
            {
                currentSong = 0;
            }
            aSource.clip = songs[currentSong];
            aSource.Play();
        }
    }
}
