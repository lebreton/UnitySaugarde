using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SavePanel : MonoBehaviour {
    public RawImage ImageSave;
    public Text Name;
    public Text Date;

    public Save save;
    public int id;

    public void Set(int id, Save save)
    {
        Texture2D tex = new Texture2D(512, 512);
        tex.LoadImage(save.RawImages);
        this.id = id;
        this.ImageSave.texture = tex;
        this.Name.text = save.Name;
        this.Date.text = save.Date.ToString();
        this.save = save;
    }

    public void SaveSelect()
    {
        SystemBackup.SaveSelect = new InfoSaveSelect(id, this.save);
        SceneManager.LoadScene("Game");
    }
}
