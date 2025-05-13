using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(SODataManager))]
public class SODataManagerEditor : Editor
{
    private ReorderableList reorderableList;

    private void OnEnable()
    {
        SerializedProperty listProperty = serializedObject.FindProperty("savedScriptableData");
        reorderableList = new ReorderableList(serializedObject, listProperty, true, true, true, true);
        reorderableList.headerHeight = EditorGUIUtility.singleLineHeight * 2 + 6f;
        
        var soManager = (SODataManager)target;

        reorderableList.drawHeaderCallback = (Rect rect) =>
        {
            // Получаем SerializedProperty для folderPath
            SerializedProperty folderPathProp = serializedObject.FindProperty("folderPath");

            GUI.Box(new Rect(rect.x - 6, rect.y, rect.width + 12, rect.height), "", EditorStyles.helpBox);

            float lineHeight = EditorGUIUtility.singleLineHeight;
            float padding = 2f;

            // Первая строка — заголовок
            EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width - 180f, lineHeight), "Saved Scriptable Data", EditorStyles.boldLabel);

            // Вторая строка — Folder Path
            EditorGUI.LabelField(new Rect(rect.x, rect.y + lineHeight + padding, 100f, lineHeight), "Folder Path");
            EditorGUI.PropertyField(new Rect(rect.x + 100f, rect.y + lineHeight + padding, rect.width - 100f, lineHeight), folderPathProp, GUIContent.none);

            // Кнопка Load
            if (GUI.Button(new Rect(rect.x + rect.width - 160f, rect.y, 50f, lineHeight), "Load"))
            {
                ((SODataManager)target).LoadAllData();
            }

            // Кнопка Save
            if (GUI.Button(new Rect(rect.x + rect.width - 105f, rect.y, 50f, lineHeight), "Save"))
            {
                ((SODataManager)target).SaveAllData();
            }

            // Поле количества элементов
            int currentSize = reorderableList.serializedProperty.arraySize;
            EditorGUI.BeginChangeCheck();
            int newSize = EditorGUI.IntField(new Rect(rect.x + rect.width - 50f, rect.y, 50f, lineHeight), currentSize);
            if (EditorGUI.EndChangeCheck() && newSize != currentSize)
            {
                reorderableList.serializedProperty.arraySize = Mathf.Max(0, newSize);
                serializedObject.ApplyModifiedProperties();
            }
        };


        reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            SerializedProperty element = reorderableList.serializedProperty.GetArrayElementAtIndex(index);
            SerializedProperty fileName = element.FindPropertyRelative("fileName");
            SerializedProperty scriptableObject = element.FindPropertyRelative("scriptableObject");

            float padding = 4f;
            float lineHeight = EditorGUIUtility.singleLineHeight;
            float y = rect.y + 2f;

            // FilePath
            Rect labelRect = new Rect(rect.x, y, 100f, lineHeight);
            Rect fileNameRect = new Rect(rect.x + 200f + padding, y, rect.width - 200f - padding, lineHeight);
            EditorGUI.LabelField(labelRect, "FileName");
            EditorGUI.PropertyField(fileNameRect, fileName, GUIContent.none);

            // ScriptableObject
            y += lineHeight + padding;
            Rect scriptableObjectRect = new Rect(rect.x, y, rect.width, lineHeight);
            EditorGUI.PropertyField(scriptableObjectRect, scriptableObject, GUIContent.none);

            // Load / Save buttons
            y += lineHeight + padding;
            Rect loadBtnRect = new Rect(rect.x, y, 50f, lineHeight);
            Rect saveBtnRect = new Rect(rect.x + 54f, y, 50f, lineHeight);

            if (GUI.Button(loadBtnRect, "Load"))
            {
                soManager.LoadData(index);
            }

            if (GUI.Button(saveBtnRect, "Save"))
            {
                soManager.SaveData(index);
            }
        };

        reorderableList.elementHeightCallback = (int index) =>
        {
            return EditorGUIUtility.singleLineHeight * 3 + 11f;
        };

        reorderableList.onAddCallback = (ReorderableList list) =>
        {
            list.serializedProperty.arraySize++;
            list.index = list.serializedProperty.arraySize - 1;
            serializedObject.ApplyModifiedProperties();
        };

        reorderableList.onRemoveCallback = (ReorderableList list) =>
        {
            if (EditorUtility.DisplayDialog("Удалить элемент?", "Вы уверены?", "Да", "Отмена"))
            {
                ReorderableList.defaultBehaviours.DoRemoveButton(list);
                serializedObject.ApplyModifiedProperties();
            }
        };
    }

    private void AddScriptableObjectToList(ScriptableObject so)
    {
        var listProp = serializedObject.FindProperty("savedScriptableData");
        int newIndex = listProp.arraySize;
        listProp.InsertArrayElementAtIndex(newIndex);

        SerializedProperty newElement = listProp.GetArrayElementAtIndex(newIndex);
        newElement.FindPropertyRelative("scriptableObject").objectReferenceValue = so;
        newElement.FindPropertyRelative("fileName").stringValue = $"{so.name}.txt";

        serializedObject.ApplyModifiedProperties();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        reorderableList.DoLayoutList();

        // Drag & Drop область
        Rect dropArea = GUILayoutUtility.GetRect(0, 50, GUILayout.ExpandWidth(true));
        GUI.Box(dropArea, "Drag & Drop ScriptableObjects Here", EditorStyles.helpBox);

        Event evt = Event.current;
        if (evt.type == EventType.DragUpdated || evt.type == EventType.DragPerform)
        {
            if (dropArea.Contains(evt.mousePosition))
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                if (evt.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();

                    foreach (var obj in DragAndDrop.objectReferences)
                    {
                        if (obj is ScriptableObject so)
                        {
                            AddScriptableObjectToList(so);
                        }
                    }
                }

                evt.Use();
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}