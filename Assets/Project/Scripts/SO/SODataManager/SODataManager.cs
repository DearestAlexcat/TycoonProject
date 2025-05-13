using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
class ScriptableObjectElement
{
    public string fileName;
    public ScriptableObject scriptableObject;
}

[CreateAssetMenu(fileName = "SODataManagerNew", menuName = "SODataManagerNew")]
public class SODataManager : ScriptableObject
{
    [SerializeField] string folderPath = "Project/Data/FolderName";
    [SerializeField] List<ScriptableObjectElement> savedScriptableData;

    ScriptableDataManager dataManager;

    private void OnEnable()
    {
        if (dataManager == null)
        {
            dataManager = new ScriptableDataManager(this);
        }
    }

    public void LoadAllData()
    {
        foreach (var item in savedScriptableData)
        {
            dataManager.LoadData($"{folderPath}\\{item.fileName}", item.scriptableObject);
        }
    }

    public void SaveAllData()
    {
        foreach (var item in savedScriptableData)
        {           
            dataManager.SaveData(Path.Combine(folderPath, item.fileName), item.scriptableObject);
        }
    }

    public void LoadData(int index)
    {
        dataManager.LoadData(Path.Combine(folderPath, savedScriptableData[index].fileName), savedScriptableData[index].scriptableObject);
    }

    public void SaveData(int index)
    {
        foreach (var item in savedScriptableData)
        {
            dataManager.SaveData(Path.Combine(folderPath, savedScriptableData[index].fileName), savedScriptableData[index].scriptableObject);
        }
    }
}

class ScriptableDataManager : SOStorageManager<ScriptableObject>
{
    public ScriptableObject data;

    public ScriptableDataManager() { }

    public ScriptableDataManager(ScriptableObject data)
    {
        this.data = data;
    }
}
