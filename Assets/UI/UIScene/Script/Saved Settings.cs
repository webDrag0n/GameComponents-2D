using System;
using UnityEngine;
using System.IO;

[Serializable]
public class SavedSettings
{
    private string jsonString;

    public int width;
    public int height;
    
    public bool fullscreen;
    // public float fov;
    // public int antiAlias;
    public bool vsync;

    public float musicVolume;
    public float effectVolume;
    public float masterVolume;

    public void Save(string filePath)
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
        jsonString = JsonUtility.ToJson(this);
        Debug.Log(jsonString);
        File.WriteAllText(filePath, jsonString);
    }

    public bool Load(string filePath)
    {
        try
        {
            SavedSettings read = JsonUtility.FromJson<SavedSettings>(File.ReadAllText(filePath));
            width = read.width;
            height = read.height;
            
            fullscreen = read.fullscreen;
            vsync = read.vsync;
            
            masterVolume = read.masterVolume;
            musicVolume = read.musicVolume;
            effectVolume = read.effectVolume;
            
            return true;
        }
        catch (FileNotFoundException)
        {
            Debug.Log("Game settings not found in: " + filePath);
        }
        catch (NullReferenceException)
        {
            Debug.Log("Game settings load failure");
        }
        return false;
    }
}