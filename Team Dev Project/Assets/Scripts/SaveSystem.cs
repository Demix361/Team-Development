using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveSystem
{
    private static string path;
    private static string directory;

    public SaveSystem(string playerName)
    {
        path = Application.persistentDataPath + Path.DirectorySeparatorChar + playerName + Path.DirectorySeparatorChar;// + "save.kek";
        directory = Application.persistentDataPath + Path.DirectorySeparatorChar + playerName;
    }

    // save unlocked levels
    public void SaveGame(bool[] levelInfo)
    {
        BinaryFormatter formatter = new BinaryFormatter();

        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        FileStream stream = new FileStream(path + "save.kek", FileMode.Create);

        GameData data = new GameData(levelInfo);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    // load unlocked levels
    public GameData LoadGame()
    {
        if (File.Exists(path + "save.kek"))
        {
            BinaryFormatter formatter = new BinaryFormatter();

            FileStream stream = new FileStream(path + "save.kek", FileMode.Open);

            GameData data = formatter.Deserialize(stream) as GameData;
            stream.Close();

            return data;
        }
        else
        {
            return null;
        }
    }

    public void SaveGems(int levelID, Dictionary<string, bool[]> gemInfo)
    {
        BinaryFormatter formatter = new BinaryFormatter();

        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        FileStream stream = new FileStream(path + levelID.ToString() + "_gems.kek", FileMode.Create);

        GemData data = new GemData(gemInfo);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public GemData LoadGems(int levelID)
    {
        if (File.Exists(path + levelID.ToString() + "_gems.kek"))
        {
            BinaryFormatter formatter = new BinaryFormatter();

            FileStream stream = new FileStream(path + levelID.ToString() + "_gems.kek", FileMode.Open);

            GemData data = formatter.Deserialize(stream) as GemData;
            stream.Close();

            return data;
        }
        else
        {
            return null;
        }
    }


    public void DeleteAllSaves()
    {
        if (Directory.Exists(directory))
        {
            Directory.Delete(directory);
        }
    }
}
