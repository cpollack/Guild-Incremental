using Sirenix.Serialization;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;

public class DataAccessor
{
    [DllImport("__Internal")]
    private static extern void SyncFiles();

    [DllImport("__Internal")]
    private static extern void WindowAlert(string message);

    public static void Save(GameData gameData)
    {
        string dataPath = string.Format("{0}/GameData.json", Application.persistentDataPath);      

        try
        {
            string jsonData = JsonUtility.ToJson(gameData);
            byte[] bytes = Encoding.ASCII.GetBytes(jsonData);

            File.WriteAllBytes(dataPath, bytes);

            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                SyncFiles();
            }
        }
        catch (Exception e)
        {
            PlatformSafeMessage("Failed to Save: " + e.Message);
        }        
    }

    public static GameData Load()
    {
        GameData gameData = null;
        string dataPath = string.Format("{0}/GameData.json", Application.persistentDataPath);

        try
        {
            if (File.Exists(dataPath))
            {
                string jsonData = Encoding.ASCII.GetString(File.ReadAllBytes(dataPath));
                gameData = JsonUtility.FromJson<GameData>(jsonData);
            }
        }
        catch (Exception e)
        {
            PlatformSafeMessage("Failed to Load: " + e.Message);
        }

        if (gameData != null) gameData.AfterLoad();
        else gameData = new GameData();
        return gameData;
    }

    private static void PlatformSafeMessage(string message)
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            WindowAlert(message);
        }
        else
        {
            Debug.Log(message);
        }
    }
}