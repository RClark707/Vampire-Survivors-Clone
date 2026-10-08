using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
[CustomEditor(typeof(UICharacterSelector))]
public class UICharacterSelectorEditor : Editor
{
    UICharacterSelector selector;

    void OnEnable()
    {
        selector = target as UICharacterSelector;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Generate Selectable Characters"))
        {
            CreateTogglesForCharacterData();
        }
    }

    public void CreateTogglesForCharacterData()
    {
        if (!selector.toggleTemplate)
        {
            Debug.LogWarning("Please assign a toggle template for the UI Character Selector.");
            return;
        }

        for (int i = selector.toggleTemplate.transform.parent.childCount - 1; i >= 0; i--)
        {
            Toggle t = selector.toggleTemplate.transform.parent.GetChild(i).GetComponent<Toggle>();
            if (t == selector.toggleTemplate) continue;
            Undo.DestroyObjectImmediate(t.gameObject);
        }

        Undo.RecordObject(selector, "Updates to UICharacterSelector.");
        selector.selectableToggles.Clear();
        CharacterStatsB[] characters = UICharacterSelector.GetAllCharacterDataAssets();

        for (int i = 0; i < characters.Length; i++)
        {
            Toggle t;
            if (i == 0)
            {
                t = selector.toggleTemplate;
                Undo.RecordObject(t, "Modifying the template.");
            }
            else
            {
                t = Instantiate(selector.toggleTemplate, selector.toggleTemplate.transform.parent);
                Undo.RegisterCreatedObjectUndo(t.gameObject, "Created a new toggle.");
            }

            Transform characterName = t.transform.Find(selector.characterNamePath);
            if (characterName && characterName.TryGetComponent(out TextMeshProUGUI tmp))
            {
                tmp.text = t.gameObject.name = characters[i].Name;
            }

            Transform characterIcon = t.transform.Find(selector.characterIconPath);
            if (characterIcon && characterIcon.TryGetComponent(out Image chrIcon))
            {
                chrIcon.sprite = characters[i].Icon;
            }

            Transform weaponIcon = t.transform.Find(selector.weaponIconPath);
            if (weaponIcon && weaponIcon.TryGetComponent(out Image wpnIcon))
            {
                wpnIcon.sprite = characters[i].StartingWeapon.icon;
            }

            selector.selectableToggles.Add(t);

            for (int j = 0; j < t.onValueChanged.GetPersistentEventCount(); j++)
            {
                if (t.onValueChanged.GetPersistentMethodName(j) == "Select")
                {
                    UnityEventTools.RemovePersistentListener(t.onValueChanged, j);
                }
            }

            UnityEventTools.AddObjectPersistentListener(t.onValueChanged, selector.Select, characters[i]);
        }

        EditorUtility.SetDirty(selector);
    }
}
