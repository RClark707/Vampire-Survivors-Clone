using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UICharacterSelector : MonoBehaviour
{
    public CharacterStats defaultCharacter;
    public static CharacterStats selectedCharacter;
    public UIStatsDisplay statsUI;

    [Header("Template Paths")]
    public Toggle toggleTemplate;
    public string characterNamePath = "CharacterName";
    public string weaponIconPath = "WeaponIcon";
    public string characterIconPath = "CharacterIcon";
    public List<Toggle> selectableToggles = new List<Toggle>();

    [Header("Description Box")]
    public TextMeshProUGUI characterFullName;
    public TextMeshProUGUI characterDescription;
    public Image selectedCharacterIcon;
    public Image selectedCharacterWeapon;

    void Start()
    {
        if (defaultCharacter) selectedCharacter = defaultCharacter;
    }

    public static CharacterStats[] GetAllCharacterDataAssets()
    {
        List<CharacterStats> characters = new List<CharacterStats>();

#if UNITY_EDITOR
        string[] allAssetPaths = AssetDatabase.GetAllAssetPaths();
        foreach (string assetPath in allAssetPaths)
        {
            if (assetPath.EndsWith(".asset"))
            {
                CharacterStats characterData = AssetDatabase.LoadAssetAtPath<CharacterStats>(assetPath);
                if (characterData != null)
                {
                    characters.Add(characterData);
                }
            }
        }
#else
    Debug.LogWarning("This function cannot be called on builds");
#endif

        return characters.ToArray();
    }

    public static CharacterStats GetData()
    {
        if (selectedCharacter) return selectedCharacter;
        else
        {
            CharacterStats[] characters = GetAllCharacterDataAssets();
            if (characters.Length > 0) return characters[Random.Range(0, characters.Length)];
        }
        return null;
    }

    public void Select(CharacterStats character)
    {
        selectedCharacter = statsUI.character = character;
        statsUI.UpdateFields();

        characterFullName.text = character.FullName;
        characterDescription.text = character.Description;
        selectedCharacterIcon.sprite = character.Icon;
        selectedCharacterWeapon.sprite = character.StartingWeapon.icon;
    }
}
