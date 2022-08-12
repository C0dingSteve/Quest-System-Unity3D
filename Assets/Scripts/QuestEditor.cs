using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[CustomEditor(typeof(Quest))]
public class QuestEditor : Editor
{
    SerializedProperty m_QuestInfoProperty;
    SerializedProperty m_QuestStatProperty;
    SerializedProperty m_QuestRewardProperty;

    List<string> m_QuestObjectiveType;
    SerializedProperty m_QuestObjectiveListProperty;

    [MenuItem("Assets/Quest", priority = 0)]
    public static void CreateQuest()
    {
        var newQuest = CreateInstance<Quest>();
        ProjectWindowUtil.CreateAsset(newQuest, "quest.asset");
    }

    void OnEnable()
    {
        m_QuestInfoProperty = serializedObject.FindProperty(nameof(Quest.information));
        m_QuestStatProperty = serializedObject.FindProperty(nameof(Quest.statBonus));
        m_QuestRewardProperty = serializedObject.FindProperty(nameof(Quest.rewards));
        
        m_QuestObjectiveListProperty = serializedObject.FindProperty(nameof(Quest.objectives));

        var lookup = typeof(Quest.QuestObjective);
        m_QuestObjectiveType = System.AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(x => x.IsClass && !x.IsAbstract && x.IsSubclassOf(lookup))
            .Select(type => type.Name)
            .ToList();
    }

    public override void OnInspectorGUI()
    {
        AddNewSection(m_QuestInfoProperty.Copy(), "Quest Info");
        AddNewSection(m_QuestStatProperty.Copy(), "Quest Stat Bonus");
        AddNewSection(m_QuestRewardProperty.Copy(), "Quest Rewards");
        
        HandleNewObjective(m_QuestObjectiveType, m_QuestObjectiveListProperty);
    }

    private void AddNewSection(SerializedProperty child, string fieldName)
    {
        var depth = child.depth;
        child.NextVisible(true);
        
        EditorGUILayout.LabelField(fieldName, EditorStyles.boldLabel);
        while (child.depth > depth)
        {
            EditorGUILayout.PropertyField(child, true);
            child.NextVisible(false);
        }
    }

    private void HandleNewObjective(List<string> list, SerializedProperty listProperty)
    {
        EditorGUILayout.LabelField("Quest Objectives", EditorStyles.boldLabel);
        int choice = EditorGUILayout.Popup("Add New Objective: ", -1, list.ToArray());

        if (choice != -1)
        {
            var newInstance = ScriptableObject.CreateInstance(list[choice]);

            AssetDatabase.AddObjectToAsset(newInstance, target);

            listProperty.InsertArrayElementAtIndex(listProperty.arraySize);
            listProperty.GetArrayElementAtIndex(listProperty.arraySize - 1)
                .objectReferenceValue = newInstance;
        }

        Editor ed = null;
        int toDelete = -1;

        for (int i = 0; i < listProperty.arraySize; i++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();

            var item = listProperty.GetArrayElementAtIndex(i);
            SerializedObject obj = new SerializedObject(item.objectReferenceValue);

            Editor.CreateCachedEditor(item.objectReferenceValue, null, ref ed);

            ed.OnInspectorGUI();
            EditorGUILayout.EndVertical();

            if (GUILayout.Button("-", GUILayout.Width(32)))
            {
                toDelete = i;
            }
            EditorGUILayout.EndHorizontal();
        }

        if (toDelete != -1)
        {
            var item = listProperty.GetArrayElementAtIndex(toDelete).objectReferenceValue;
            DestroyImmediate(item, true);
            listProperty.DeleteArrayElementAtIndex(toDelete);
        }
        serializedObject.ApplyModifiedProperties();
    }
}
#endif
