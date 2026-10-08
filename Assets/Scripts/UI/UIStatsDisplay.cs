using System.Reflection;
using System.Text;
using TMPro;
using UnityEditor;
using UnityEngine;

public class UIStatsDisplay : MonoBehaviour
{
    public Player player;
    public CharacterStatsB character;
    public bool updateInEditor = false;
    TextMeshProUGUI statNamesDisplay, statValuesDisplay;

    void OnEnable()
    {
        UpdateStatDisplay();
    }

    private void OnDrawGizmosSelected()
    {
        if (updateInEditor) UpdateStatDisplay();
    }

    public CharacterStatsB.Stats GetDisplayedStats()
    {
        if (player) return player.ActualStats;
        else if (character) return character.stats;
        return new CharacterStatsB.Stats();
    }

    // TODO: Make the Stats & Values on a single line!
    public void UpdateStatDisplay()
    {
        if (!player && !character) return;

        if (!statNamesDisplay) statNamesDisplay = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        if (!statValuesDisplay) statValuesDisplay = transform.GetChild(1).GetComponent<TextMeshProUGUI>();

        // StringBuilder makes the manipulation run quicker
        StringBuilder names = new StringBuilder();
        StringBuilder values = new StringBuilder();
        FieldInfo[] fields = typeof(CharacterStatsB.Stats).GetFields(BindingFlags.Public | BindingFlags.Instance);
        foreach (FieldInfo field in fields)
        {
            // render the stat names
            names.AppendLine(ObjectNames.NicifyVariableName(field.Name));

            // get the stat values
            object val = field.GetValue(GetDisplayedStats());
            float fval = val is int ? (int)val : (float)val;

            PropertyAttribute attribute = (PropertyAttribute)PropertyAttribute.GetCustomAttribute(field, typeof(PropertyAttribute));
            if (attribute != null && field.FieldType == typeof(float))
            {
                float percentage = Mathf.Round(fval * 100f - 100f);

                if (Mathf.Approximately(percentage, 0f))
                {
                    values.Append('-').Append('\n');
                }
                else
                {
                    if (percentage > 0f)
                    {
                        values.Append('+');
                    }
                    values.Append(percentage).Append('%').Append('\n');
                }
            }
            else
            {
                if (Mathf.Approximately(fval, 0f))
                {
                    values.Append('-').Append('\n');
                }
                else
                {
                    values.Append(fval).Append('\n');
                }
            }

            statNamesDisplay.text = names.ToString();
            statValuesDisplay.text = values.ToString();
        }
    }

    void Reset()
    {
        player = FindAnyObjectByType<Player>();
    }
}
