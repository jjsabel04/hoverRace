using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class radioManager : MonoBehaviour
{

    private int currentRadio;
    [Range(0,1)]public float maxRadioVolume;

    // Start is called before the first frame update
    void Start()
    {
        currentRadio = Random.Range(0,transform.childCount);
        transform.GetChild(currentRadio).GetComponent<AudioSource>().volume = maxRadioVolume;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            transform.GetChild(currentRadio).GetComponent<AudioSource>().volume = 0;
            if (currentRadio-1 >= 0)
            {
                currentRadio--;
            }
            else
            {
                currentRadio = transform.childCount-1;
            }
            transform.GetChild(currentRadio).GetComponent<AudioSource>().volume = maxRadioVolume;
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            transform.GetChild(currentRadio).GetComponent<AudioSource>().volume = 0;
            if (currentRadio+1 <= transform.childCount-1)
            {
                currentRadio++;
            }
            else
            {
                currentRadio = 0;
            }
            transform.GetChild(currentRadio).GetComponent<AudioSource>().volume = maxRadioVolume;
        }
    }
}
