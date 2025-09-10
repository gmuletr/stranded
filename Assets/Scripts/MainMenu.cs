using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class MainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject optionsUI;
    public GameObject[] options;

    public AudioMixer mixer;
    void Start()
    {
        options = GameObject.FindGameObjectsWithTag("OptionsMenu");
        optionsUI = options[0];
    }
    public void LoadScene(string sceneToLoad)
    {
        Singleton.instance.sceneToLoad = sceneToLoad;
        SceneManager.LoadScene(sceneToLoad);
    }

   
    public void ExitGame()
    {
        Application.Quit();
    }

    public void OpenOptions()
    {
        optionsUI.GetComponent<Canvas>().enabled = true; 
       

    }

    public void ExitOptions()
    {
        Destroy(gameObject.transform.root);
    }
}
