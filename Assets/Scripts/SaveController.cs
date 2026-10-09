using System.IO;
using UnityEngine;

public class SaveController
{
    public class GameData
    {
        public float coins;
    }

    const string SAVE_FILE_NAME = "SaveData.json";

    static GameData lastLoadedGameData;
    public static GameData LastLoadedGameData
    {
        get
        {
            if (lastLoadedGameData == null) Load();
            return lastLoadedGameData;
        }
    }

    public static string GetSavePath()
    {
        return string.Format("{0}/{1}", Application.persistentDataPath, SAVE_FILE_NAME);
    }

    public static void Save(GameData data = null)
    {
        if (data == null)
        {
            if (lastLoadedGameData == null) Load();
            data = lastLoadedGameData;
        }
        File.WriteAllText(GetSavePath(), JsonUtility.ToJson(data));
    }

    public static GameData Load(bool usePreviousLoadIfAvailable = false)
    {
        if (usePreviousLoadIfAvailable && lastLoadedGameData != null)
        {
            return lastLoadedGameData;
        }

        string path = GetSavePath();
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            lastLoadedGameData = JsonUtility.FromJson<GameData>(json);
            if (lastLoadedGameData == null) lastLoadedGameData = new GameData();
        }
        else
        {
            lastLoadedGameData = new GameData();
        }
        return lastLoadedGameData;
    }
}
