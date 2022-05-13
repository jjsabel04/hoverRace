using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Check For Any Key Press And Call Function
    void Update()
    {
        if (Input.anyKeyDown)
        {
            LoadScene();
        }
    }
    
    void LoadScene()
    {
        SceneManager.LoadScene(1);
    }
}
