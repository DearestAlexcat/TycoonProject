using System;
using System.IO;
using System.Reflection;
using UnityEngine;

public abstract class SOStorageManager<T> where T : ScriptableObject
{
    public void TransferDataToFields(ScriptableObject source, T target)
    {
        // Получаем все публичные поля типа T
        var fields = target.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
      
        foreach (var field in fields)
        {
            var value = field.GetValue(source);     // Получаем значение поля из объекта source
            field.SetValue(target, value);          // Устанавливаем значение в поле объекта target
        }
    }

    public void LoadData(string filePath, T target)
    {
        string fp = Path.Combine(Application.dataPath, filePath);

        if (string.IsNullOrEmpty(fp))
        {
            Debug.LogError("File path is not set.");
            return;
        }
        
        if (File.Exists(fp))
        {
            string json = File.ReadAllText(fp);
            var loadedData = ScriptableObject.CreateInstance(target.GetType());
            JsonUtility.FromJsonOverwrite(json, loadedData);

            TransferDataToFields(loadedData, target);

            Debug.Log("<color=yellow>Data loaded from </color>" + filePath);
        }
        else
        {
            Debug.LogError("File not found at " + fp);
        }
    }

    public void SaveData(string filePath, T data)
    {
        string fp = Path.Combine(Application.dataPath, filePath);

        if (string.IsNullOrEmpty(fp))
        {
            Debug.LogError("File path is not set.");
            return;
        }

        string json = JsonUtility.ToJson(data, true);       // Сериализация данных в JSON строку
        File.WriteAllText(fp, json);                        // Запись JSON строки в файл

        Debug.Log("<color=yellow>Data saved to </color>" + fp);
    }
}