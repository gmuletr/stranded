using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameCinematica : MonoBehaviour
{
    void LoadTitle()
    {
        SceneManager.LoadScene("TitleScene");
    }
}
