using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SkipCinematica : MonoBehaviour
{
    public string nextScene;
    
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(0))
        {
            LoadScene(nextScene);
        }
    }

    public void LoadScene(string sceneToLoad)
    {
        Singleton.instance.sceneToLoad = sceneToLoad;
        SceneManager.LoadScene(sceneToLoad);
    }
}
