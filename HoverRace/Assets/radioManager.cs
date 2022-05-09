using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class radioManager : MonoBehaviour
{

    private int currentRadio;

    // Start is called before the first frame update
    void Start()
    {
        currentRadio = Random.Range(0,transform.childCount);
        transform.GetChild(currentRadio).GetComponent<AudioSource>().volume = 1;
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
            transform.GetChild(currentRadio).GetComponent<AudioSource>().volume = 1;
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
            transform.GetChild(currentRadio).GetComponent<AudioSource>().volume = 1;
        }
    }
}
