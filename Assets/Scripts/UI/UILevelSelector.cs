using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UILevelSelector : MonoBehaviour
{
    public UISceneDataDisplay statsUI;

    public static int selectedLevel = -1;
    public static SceneData currentLevel;
    public List<SceneData> levels = new List<SceneData>();

    [Header("Template")]
    public Toggle toggleTemplate;
    public string levelNamePath = "LevelName";
    public string levelNumberPath = "LevelNumber";
    public string levelDescriptionPath = "LevelDescription";
    public string levelImagePath = "LevelImage";
    public List<Toggle> selectableToggles = new List<Toggle>();

    public static BuffData globalBuff;

    public static bool globalBuffAffectsPlayer = false, globalBuffAffectsEnemies = false;

    public const string MAP_NAME_FORMAT = "^(Level .*?) ?- ?(.*)$";

    [System.Serializable]
    public class SceneData
    {
        public SceneAsset scene;

        [Header("UI Display")]
        public string displayName;
        public string label;
        [TextArea] public string description;
        public Sprite icon;

        [Header("Modifiers")]
        public CharacterStatsB.Stats playerModifier;
        public Enemy.Stats enemyModifier;
        [Min(-1)] public float timeLimit = 0f, clockSpeed = 1f;
        [TextArea] public string extraNotes = "--";
    }

    public static SceneAsset[] GetAllMaps()
    {
        List<SceneAsset> maps = new List<SceneAsset>();

#if UNITY_EDITOR
        string[] allAssetPaths = AssetDatabase.GetAllAssetPaths();
        foreach (string path in allAssetPaths)
        {
            if (path.EndsWith(".unity"))
            {
                SceneAsset map = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
                if (map != null && Regex.IsMatch(map.name, MAP_NAME_FORMAT))
                {
                    maps.Add(map);
                }
            }
        }
#else
Debug.LogWarning("This function cannot be called on builds.");
#endif
        maps.Reverse();
        return maps.ToArray();
    }

    public void SceneChange(string name)
    {
        SceneManager.LoadScene(name);
        Time.timeScale = 1f;
    }

    public void LoadSelectedLevel()
    {
        if (selectedLevel >= 0)
        {
            SceneManager.LoadScene(levels[selectedLevel].scene.name);
            currentLevel = levels[selectedLevel];
            selectedLevel = -1;
            Time.timeScale = 1f;
        }
        else
        {
            Debug.LogWarning("No level was selected");
        }
    }

    public void Select(int sceneIndex)
    {
        selectedLevel = sceneIndex;
        statsUI.UpdateFields();
        globalBuff = GenerateGlobalBuffData();
        globalBuffAffectsPlayer = globalBuff && IsModifierEmpty(globalBuff.variations[0].playerModifier);
        globalBuffAffectsEnemies = globalBuff && IsModifierEmpty(globalBuff.variations[0].enemyModifier);
    }

    public BuffData GenerateGlobalBuffData()
    {
        BuffData bd = ScriptableObject.CreateInstance<BuffData>();
        bd.name = "Global Level Buff";
        bd.variations[0].damagePerSecond = 0f;
        bd.variations[0].duration = 0f;
        bd.variations[0].playerModifier = levels[selectedLevel].playerModifier;
        bd.variations[0].enemyModifier = levels[selectedLevel].enemyModifier;
        return bd;
    }

    private static bool IsModifierEmpty(object obj)
    {
        Type type = obj.GetType();
        FieldInfo[] fields = type.GetFields();
        float sum = 0;
        foreach (FieldInfo f in fields)
        {
            object val = f.GetValue(obj);
            if (val is int) sum += (int)val;
            else if (val is float) sum += (float)val;
        }

        return Mathf.Approximately(sum, 0f);
    }
}
