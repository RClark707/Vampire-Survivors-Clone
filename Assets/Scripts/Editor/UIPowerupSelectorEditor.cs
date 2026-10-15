using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
[CustomEditor(typeof(UIPowerupSelector))]
public class UIPowerupSelectorEditor : Editor // we should make a base class for all these
{
    UIPowerupSelector selector;

    private void OnEnable()
    {
        selector = target as UIPowerupSelector;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Generate Selectable Powerups"))
        {
            CreateTogglesForPowerupData();
        }
    }

    public void CreateTogglesForPowerupData()
    {
        if (!selector.toggleTemplate)
        {
            Debug.LogWarning("Please assign a toggle template for the UI Powerup Selector.");
            return;
        }

        Toggle template = selector.toggleTemplate;

        // clear all non-template toggle
        for (int i = template.transform.parent.childCount - 1; i >= 0; i--)
        {
            Toggle tog = template.transform.parent.GetChild(i).GetComponent<Toggle>();
            if (tog == template) continue;
            Undo.DestroyObjectImmediate(tog.gameObject);
        }

        Undo.RecordObject(selector, "Updates to UIPowerupSelector.");

        PowerupStats[] powerups = UIPowerupSelector.GetAllPowerups();

        // make new toggles
        for (int i = 0; i < powerups.Length; i++)
        {
            Toggle tog;
            if (i == 0)
            {
                tog = template;
                Undo.RecordObject(tog, "Modifying the template.");
            }
            else
            {
                tog = Instantiate(template, template.transform.parent);
                Undo.RegisterCreatedObjectUndo(tog.gameObject, "Created a new toggle.");
            }

            tog.group = selector.GetComponent<ToggleGroup>();

            Transform nameTransform = tog.transform.Find(selector.powerupNamePath);
            if (nameTransform && nameTransform.TryGetComponent(out TextMeshProUGUI nameText))
            {
                nameText.text = tog.gameObject.name = powerups[i].name;
            }

            Transform iconTransform = tog.transform.Find(selector.powerupIconPath);
            if (iconTransform && iconTransform.TryGetComponent(out Image icon))
            {
                icon.sprite = powerups[i].icon;
            }

            Transform tickBoxesContainer = tog.transform.Find(selector.tickboxContainerPath); // everything should prolly use these paths
            if (tickBoxesContainer != null)
            {
                Transform boxTemplate = tickBoxesContainer.childCount > 0 ? tickBoxesContainer.GetChild(0) : null;

                if (boxTemplate != null)
                {
                    // clear all boxes
                    for (int j = tickBoxesContainer.childCount - 1; j >= 1; j--)
                    {
                        Undo.DestroyObjectImmediate(tickBoxesContainer.GetChild(j).gameObject);
                    }

                    int maxLevel = powerups[i].maxLevel;

                    // make new boxes
                    for (int j = 0; j < maxLevel; j++)
                    {
                        Transform box;
                        if (j == 0)
                        {
                            box = boxTemplate;
                            Undo.RecordObject(box, "Modifying tick box template.");
                        }
                        else
                        {
                            box = Instantiate(boxTemplate, tickBoxesContainer);
                            Undo.RegisterCreatedObjectUndo(box.gameObject, "Created a new tick box.");
                        }

                        box.gameObject.name = $"Box ({j + 1})";

                        Transform tick = box.Find(selector.tickPath);
                        if (tick != null)
                        {
                            tick.gameObject.SetActive(false);
                        }
                    }
                }
                else
                {
                    Debug.LogWarning($"No tick box template found beneath {selector.tickboxContainerPath} for {powerups[i].name}");
                }
            }

            for (int j = tog.onValueChanged.GetPersistentEventCount() - 1; j >= 0; j--)
            {
                if (tog.onValueChanged.GetPersistentMethodName(j) == "Select")
                {
                    UnityEventTools.RemovePersistentListener(tog.onValueChanged, j);
                }
            }

            UnityEventTools.AddObjectPersistentListener(tog.onValueChanged, selector.Select, powerups[i]);
        }

        EditorUtility.SetDirty(selector);

        Debug.Log($"Successfully generated {powerups.Length} powerup templates!");
    }
}
