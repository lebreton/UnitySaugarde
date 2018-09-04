using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPlay : MonoBehaviour {

    public RenderTexture saveImg;
    public Save _Save;
    public int id = -1;

    public GameObject Player;

    public void Awake()
    {
        if (SystemBackup.SaveSelect == null)
        {
            _Save = new Save("Test", new DateTime());
            id = -1;
        }
        else
        {
            _Save = SystemBackup.SaveSelect.SaveSelect;
            id = SystemBackup.SaveSelect.id;
        }

       Player.transform.position = _Save.GetValue<Vector3>("Player");

        Debug.Log(_Save.GetValue<Vector3>("Player"));

    }

    public void Save()
    {
        _Save.SetValue<Vector3>("Player", Player.transform.position);
    }

    Texture2D toTexture2D(RenderTexture rTex)
    {
        Texture2D tex = new Texture2D(512, 512, TextureFormat.RGB24, false);
        RenderTexture.active = rTex;
        tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        tex.Apply();
        return tex;
    }

    public void Quite()
    {
        if(_Save!=null)
            _Save.WriteSave(toTexture2D(saveImg).EncodeToPNG());
        SceneManager.LoadScene("MainMenu");
    }
}
