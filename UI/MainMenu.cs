using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    public UIOption uiOption;
    public UILoad uiLoad;

    public const string BuildID = "CADDA9A333A7541191CC793F9B21F";

    public void Awake()
    {
        SystemBackup.Load(BuildID);
    }

    public void Play()
    {
        SceneManager.LoadScene("Game");
    }

    public void Load()
    {
        uiLoad.Show();
    }

    public void Option()
    {
        uiOption.Show();
    }

    public void Quite()
    {

    }
}
