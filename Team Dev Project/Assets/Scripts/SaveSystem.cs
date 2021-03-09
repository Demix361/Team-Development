using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveSystem
{
    private static string path;
    private static string directory;

    public SaveSystem(string playerName)
    {
        path = Application.persistentDataPath + Path.DirectorySeparatorChar + playerName + Path.DirectorySeparatorChar + "save.kek";
        directory = Application.persistentDataPath + Path.DirectorySeparatorChar + playerName;
    }

    public void SaveGame(bool[] levelInfo)
    {
        BinaryFormatter formatter = new BinaryFormatter();

        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        FileStream stream = new FileStream(path, FileMode.Create);

        GameData data = new GameData(levelInfo);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public GameData LoadGame()
    {
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();

            FileStream stream = new FileStream(path, FileMode.Open);

            GameData data = formatter.Deserialize(stream) as GameData;
            stream.Close();

            return data;
        }
        else
        {
            return null;
        }
    }
}
