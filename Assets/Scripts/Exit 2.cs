using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Exit : MonoBehaviour
{
    public string sceneToLoad;
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Exit") {
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}
