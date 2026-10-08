using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UICharacterSelector : MonoBehaviour
{
    public CharacterStatsB defaultCharacter;
    public static CharacterStatsB selectedCharacter;
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

    public static CharacterStatsB[] GetAllCharacterDataAssets()
    {
        List<CharacterStatsB> characters = new List<CharacterStatsB>();

#if UNITY_EDITOR
        string[] allAssetPaths = AssetDatabase.GetAllAssetPaths();
        foreach (string assetPath in allAssetPaths)
        {
            if (assetPath.EndsWith(".asset"))
            {
                CharacterStatsB characterData = AssetDatabase.LoadAssetAtPath<CharacterStatsB>(assetPath);
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

    public static CharacterStatsB GetData()
    {
        if (selectedCharacter) return selectedCharacter;
        else
        {
            CharacterStatsB[] characters = GetAllCharacterDataAssets();
            if (characters.Length > 0) return characters[Random.Range(0, characters.Length)];
        }
        return null;
    }

    public void Select(CharacterStatsB character)
    {
        selectedCharacter = statsUI.character = character;
        statsUI.UpdateStatDisplay();

        characterFullName.text = character.FullName;
        characterDescription.text = character.Description;
        selectedCharacterIcon.sprite = character.Icon;
        selectedCharacterWeapon.sprite = character.StartingWeapon.icon;
    }
}
