using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

[CustomEditor(typeof(WeaponStatsB))]
public class WeaponStatsEditor : Editor
{
    WeaponStatsB weaponStats;
    string[] weaponSubtypes;
    int selectedWeaponSubtype;

    void OnEnable()
    {
        weaponStats = (WeaponStatsB)target;

        System.Type baseType = typeof(WeaponB);
        List<System.Type> subTypes = System.AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => baseType.IsAssignableFrom(p) && p != baseType)
            .ToList();

        List<string> subTypeString = subTypes.Select(t => t.Name).ToList();
        subTypeString.Insert(0, "None");
        weaponSubtypes = subTypeString.ToArray();

        selectedWeaponSubtype = Math.Max(0, Array.IndexOf(weaponSubtypes, weaponStats.behavior));
    }

    public override void OnInspectorGUI()
    {
        selectedWeaponSubtype = EditorGUILayout.Popup("Behavior", Math.Max(0, selectedWeaponSubtype, weaponSubtypes));

        if (selectedWeaponSubtype > 0)
        {
            weaponStats.behavior = weaponSubtypes[selectedWeaponSubtype].ToString();
            EditorUtility.SetDirty(weaponStats);
            DrawDefaultInspector();
        }
    }
}
