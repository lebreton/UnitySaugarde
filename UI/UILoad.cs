using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class UILoad : WindowUI
{

    [SerializeField]
    private GameObject SavePanel;

    [SerializeField]
    private Transform Content;

    public void Back()
    {
        this.Close();
    }

    public override void Ini()
    {
        Save[] Saves = SystemBackup.GetSaves();
        int id = 0;
        foreach (Save save in Saves)
        {
            SavePanel p = Instantiate(SavePanel).GetComponent<SavePanel>();
            p.gameObject.SetActive(true);
            
            p.Set(id, save);
            p.transform.SetParent(Content);
            id++;
        }
    }
}