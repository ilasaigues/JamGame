using System.IO;
using UnityEngine;

namespace AstralCore
{
    public class SaveManager : MonoBehaviour
    {
        string AppDataPath = Application.persistentDataPath;

        public void Save<T>(T obj, string savePath)
        {
            var serialized = JsonUtility.ToJson(obj, true);
            var filePath = Path.Combine(AppDataPath, savePath);
            File.WriteAllText(filePath, serialized);
        }

        public T Load<T>(string loadPath)
        {
            string filePath = Path.Combine(Application.persistentDataPath, loadPath);
            if (File.Exists(filePath))
            {
                string jsonString = File.ReadAllText(filePath);
                T deserialized = JsonUtility.FromJson<T>(jsonString);
                return deserialized;
            }
            throw new FileNotFoundException(filePath);
        }
    }
}
