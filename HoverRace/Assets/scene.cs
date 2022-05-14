using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class scene : MonoBehaviour
{

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            SceneManager.LoadScene(2);
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            SceneManager.LoadScene(3);
        }
    }
}
