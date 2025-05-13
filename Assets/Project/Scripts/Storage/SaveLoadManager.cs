using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace IdleTycoon
{
    [Serializable]
    public class SaveFileData
    {
        public List<Entry> entries = new();

        [Serializable]
        public class Entry
        {
            public string key;
            public string jsonData;
        }

        public void Set(string key, string json)
        {
            var entry = entries.Find(e => e.key == key);
            if (entry != null)
            {
                entry.jsonData = json; // Обновляет существующую запись
            }
            else
            {
                entries.Add(new Entry { key = key, jsonData = json }); // Добавляет новую запись
            }
        }

        public string Get(string key)
        {
            return entries.Find(e => e.key == key)?.jsonData;
        }

        public bool Contains(string key) => entries.Exists(e => e.key == key);

        public void Remove(string key) => entries.RemoveAll(e => e.key == key);
    }

    public class SaveLoadManager : MonoBehaviour
    {
#if UNITY_EDITOR
        static readonly string FilePath = Path.Combine("C:\\Users\\sasha\\Desktop\\", "save1.json");
#else
    static readonly string FilePath = Path.Combine(Application.persistentDataPath, "save1.json");
#endif

        static SaveFileData cache;

        static SaveFileData LoadFile()
        {
            if (cache != null) return cache;

            if (!File.Exists(FilePath))
            {
                cache = new SaveFileData();
                return cache;
            }

            string json = File.ReadAllText(FilePath);
            cache = JsonUtility.FromJson<SaveFileData>(json) ?? new SaveFileData();
            return cache;
        }

        static void SaveFile()
        {
            string json = JsonUtility.ToJson(cache, true);
            File.WriteAllText(FilePath, json);
        }

        // ================================================================================================================

        public static void Save<T>(StorageKeys key, T data) where T : class
        {
            Save<T>(key.ToString(), data);
        }

        public static void Save<T>(string key, T data) where T : class
        {
            var file = LoadFile();
            string jsonData = JsonUtility.ToJson(data);
            file.Set(key, jsonData);
            SaveFile();
        }

        public static T Load<T>(StorageKeys key) where T : class
        {
            return Load<T>(key.ToString());
        }

        public static T Load<T>(string key) where T : class
        {
            var file = LoadFile();
            if (!file.Contains(key)) return default;

            string jsonData = file.Get(key);
            return JsonUtility.FromJson<T>(jsonData);
        }

        public static void Delete(string key)
        {
            var file = LoadFile();
            file.Remove(key);
            SaveFile();
        }

        public static bool Exists(string key)
        {
            var file = LoadFile();
            return file.Contains(key);
        }
    }
}