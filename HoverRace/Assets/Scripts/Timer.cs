using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] private float time;
    [SerializeField] private TextMeshProUGUI text;
    
    void Update()
    {
        time += Time.deltaTime;
        DisplayTime(time);
    }
    
    void DisplayTime(float timeToDisplay)
    {
        float minutes = Mathf.FloorToInt(timeToDisplay / 60); 
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        float milliseconds = timeToDisplay % 1 * 10000;
        text.text = $"{minutes:00}:{seconds:00}.<size=42.21>{milliseconds:0000}";
        
        
    }
}
