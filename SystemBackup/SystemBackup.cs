using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;

public class InfoSaveSelect
{
    public int id;
    public Save SaveSelect;

    public InfoSaveSelect(int id, Save saveSelect)
    {
        this.id = id;
        this.SaveSelect = saveSelect;
    }
}

public static class SystemBackup {

    public static string PathSave;
    public static string CompanyName;
    public static string ProductName;
    public static string BuildID;

    private static int Countsave = 0;

    public static InfoSaveSelect SaveSelect { get; set; }

    public static void Load(string BuildID)
    {
        RuntimePlatform platform = Application.platform;
        string MyDocuments = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
        if (platform == RuntimePlatform.WindowsPlayer || platform == RuntimePlatform.WindowsEditor)
        {
            SystemBackup.BuildID = BuildID;
            SystemBackup.ProductName = Application.productName;
            SystemBackup.CompanyName = Application.companyName;
            SystemBackup.PathSave = MyDocuments + @"\" + CompanyName + @"\" + ProductName + @"\save";

            if (!Directory.Exists(SystemBackup.PathSave))
                Directory.CreateDirectory(SystemBackup.PathSave);

            Debug.Log(SystemBackup.PathSave);
        }
        else
        {
            throw new Exception("SystemBackup Incompatible :" + platform.ToString());
        }
    }

    public static Save[] GetSaves()
    {
        string [] PathFilesaves =  Directory.GetFiles(SystemBackup.PathSave);
        List<Save> SaveFiles = new List<Save>();

        foreach(string file in PathFilesaves)
        {
            FileInfo Info = new FileInfo(file);
            Debug.Log(Info.Name);
            if (Info.Extension == ".save")
                SaveFiles.Add(new Save(File.ReadAllBytes(file)));
        }

        Countsave = SaveFiles.ToArray().Length-1;

        return SaveFiles.ToArray();
    }

    public static void WriteSave(this Save Save, int id = -1)
    {
        if (id == -1)
            id = Countsave;

        File.WriteAllBytes(SystemBackup.PathSave + @"\" + Save.Name + "-" + id + ".save", Save.GetData());
        Countsave++;
    }

}
