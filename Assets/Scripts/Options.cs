using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class Options : MonoBehaviour
{
    public GameObject controlsGroup;
    public GameObject optionsGroup;

    public GameObject keyboardImage;
    public GameObject controllerImage;

    public GameObject tipsGroup;

    public AudioMixer mixer;

    private void Start()
    {
        
        DontDestroyOnLoad(gameObject);
    }
    public void QuitOptions()
    {
        gameObject.GetComponent<Canvas>().enabled = false;
    }

    public void DisplayOptions()
    {
        controlsGroup.SetActive(false);
        optionsGroup.SetActive(true);
    }

    public void DisplayControls()
    {
        controlsGroup.SetActive(true);
        optionsGroup.SetActive(false);
    }

    public void DisplayTips()
    {
        tipsGroup.GetComponent<Canvas>().enabled = true;
    }

    public void QuitTips()
    {
        tipsGroup.GetComponent<Canvas>().enabled = false;
    }

    public void ToggleFullscreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
    }
    public void SwitchControls()
    {
        keyboardImage.SetActive(!keyboardImage.activeInHierarchy);
        controllerImage.SetActive(!controllerImage.activeInHierarchy);
    }


    public void SetMasterVolume (float sliderValue)
    {
        mixer.SetFloat("MasterVolume", Mathf.Log10(sliderValue) * 20);
    }
    public void SetAmbientVolume(float sliderValue)
    {
        mixer.SetFloat("AmbientVolume", Mathf.Log10(sliderValue) * 20);
    }

    public void SetSFXVolume(float sliderValue)
    {
        mixer.SetFloat("SFXVolume", Mathf.Log10(sliderValue) * 20);
    }
}
