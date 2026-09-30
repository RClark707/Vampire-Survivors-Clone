using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UIInventoryIconDisplay))]
public class UIInventoryIconsDisplayEditor : Editor
{
    UIInventoryIconDisplay display;
    int targetedItemListIndex = 0;
    string[] itemListOptions;

    void OnEnable()
    {
        display = target as UIInventoryIconDisplay;

        Type playerInventoryType = typeof(PlayerInventoryController);

        FieldInfo[] fields = playerInventoryType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        List<string> slotListNames = fields
            .Where(field => field.FieldType.IsGenericType &&
            field.FieldType.GetGenericTypeDefinition() == typeof(List<>) &&
            field.FieldType.GetGenericArguments()[0] == typeof(PlayerInventoryController.Slot))
            .Select(field => field.Name)
            .ToList();

        slotListNames.Insert(0, "None");
        itemListOptions = slotListNames.ToArray();

        targetedItemListIndex = Math.Max(0, Array.IndexOf(itemListOptions, display.targetedItemList));
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUI.BeginChangeCheck();

        targetedItemListIndex = EditorGUILayout.Popup("Targeted Item List", Mathf.Max(0, targetedItemListIndex), itemListOptions);

        if (EditorGUI.EndChangeCheck())
        {
            display.targetedItemList = itemListOptions[targetedItemListIndex].ToString();
            EditorUtility.SetDirty(display);
        }

        if (GUILayout.Button("Generate Icons")) RegenerateIcons();
    }

    void RegenerateIcons() // this lets us quickly modify the UI without copying and pasting a bunch
    {
        display = target as UIInventoryIconDisplay;

        Undo.RegisterCompleteObjectUndo(display, "Regenerate Icons");

        if (display.slots.Length > 0)
        {
            foreach (GameObject go in display.slots)
            {
                if (!go) continue;

                if (go != display.slotTemplate)
                {
                    Undo.DestroyObjectImmediate(go);
                }
            }
        }

        // destroy everything except the slot template
        for (int i = 0; i < display.transform.childCount; i++)
        {
            if (display.transform.GetChild(i).gameObject == display.slotTemplate) continue;
            Undo.DestroyObjectImmediate(display.transform.GetChild(i).gameObject);
            i--;
        }

        if (display.maxSlots <= 0) return;

        display.slots = new GameObject[display.maxSlots];
        display.slots[0] = display.slotTemplate;
        for (int i = 1; i < display.slots.Length; i++)
        {
            display.slots[i] = Instantiate(display.slotTemplate, display.transform);
            display.slots[i].name = display.slotTemplate.name;
        }
    }
}

