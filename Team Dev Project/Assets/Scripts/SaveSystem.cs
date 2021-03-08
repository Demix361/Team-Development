using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveSystem
{
    //private static string path = Application.persistentDataPath + "/save.kek";
    private string path = "/Users/said/Documents/save.kek";

    public void SaveGame (bool[] levelInfo)
    {
        //string path = Application.persistentDataPath + Path.PathSeparator + "save.kek";
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);

        GameData data = new GameData(levelInfo);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public GameData LoadGame ()
    {
        //string path = Application.persistentDataPath + Path.PathSeparator + "save.kek";

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
