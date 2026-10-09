using System;
using System.Reflection;
using System.Text;
using TMPro;
using UnityEngine;

public class UISceneDataDisplay : UIPropertyDisplay
{
    public UILevelSelector levelSelector;
    TextMeshProUGUI extraStageInfo;

    public override object GetReadObject()
    {
        if (levelSelector && UILevelSelector.selectedLevel >= 0)
        {
            return levelSelector.levels[UILevelSelector.selectedLevel];
        }
        else
        {
            return new UILevelSelector.SceneData();
        }
    }

    public override void UpdateFields()
    {
        if (!propertyNames) propertyNames = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        if (!propertyValues) propertyValues = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        if (!extraStageInfo) extraStageInfo = transform.GetChild(2).GetComponent<TextMeshProUGUI>();

        StringBuilder[] allData = GetProperties(
            BindingFlags.Public | BindingFlags.Instance,
            "UILevelSelector+SceneData"
            );

        UILevelSelector.SceneData dat = (UILevelSelector.SceneData)GetReadObject();

        allData[0].AppendLine("Move Speed")
            .AppendLine("Gold Bonus")
            .AppendLine("Luck Bonus")
            .AppendLine("XP Bonus")
            .AppendLine("Enemy Health");

        Type characterStats = typeof(CharacterStatsB.Stats);
        ProcessValue(dat.playerModifier.moveSpeed, allData[1], characterStats.GetField("moveSpeed"));
        ProcessValue(dat.playerModifier.greed, allData[1], characterStats.GetField("greed"));
        ProcessValue(dat.playerModifier.luck, allData[1], characterStats.GetField("luck"));
        ProcessValue(dat.playerModifier.growth, allData[1], characterStats.GetField("growth"));

        Type enemyStats = typeof(Enemy.Stats);
        ProcessValue(dat.enemyModifier.maxHealth, allData[1], enemyStats.GetField("maxHealth"));

        if (propertyNames) propertyNames.text = allData[0].ToString();
        if (propertyValues) propertyValues.text = allData[1].ToString();
        // see ProcessValues for setting the extra stage info
    }

    protected override bool IsFieldShown(FieldInfo field)
    {
        switch (field.Name)
        {
            default:
                return false;
            case "timeLimit":
            case "clockSpeed":
            case "moveSpeed":
            case "greed":
            case "luck":
            case "growth":
            case "maxHealth":
                return true;
        }
    }

    protected override StringBuilder ProcessName(string name, StringBuilder output, FieldInfo field)
    {
        if (field.Name == "extraNotes") return output;
        return base.ProcessName(name, output, field);
    }

    protected override StringBuilder ProcessValue(object value, StringBuilder output, FieldInfo field)
    {
        float fval;
        switch (field.Name)
        {
            case "timeLimit":
                fval = value is int ? (int)value : (float)value;
                if (Mathf.Approximately(fval, 0f))
                {
                    output.Append(DASH).Append('\n');
                }
                else
                {
                    string minutes = Mathf.FloorToInt(fval / 60).ToString();
                    string seconds = (fval % 60).ToString();
                    if (fval % 60 < 10)
                    {
                        seconds += "0";
                    }
                    output.Append(minutes).Append(':').Append(seconds).Append('\n');
                }
                return output;

            case "clockSpeed":
                fval = value is int ? (int)value : (float)value;
                output.Append(fval).Append('x').Append('\n');
                return output;
            case "maxHealth":
            case "moveSpeed":
            case "greed":
            case "luck":
            case "growth":
                fval = value is int ? (int)value : (float)value;
                float percentage = Mathf.Round(fval * 100);

                if (Mathf.Approximately(percentage, 0f))
                {
                    output.Append(DASH).Append('\n');
                }
                else
                {
                    if (percentage > 0f)
                        output.Append('+');
                    output.Append(percentage).Append('%').Append('\n');
                }
                return output;
            case "extraNotes":
                if (value == null) return output;
                string msg = value.ToString();
                extraStageInfo.text = string.IsNullOrWhiteSpace(msg) ? DASH : msg; // this is where it handles the extra notes
                return output;
        }

        return base.ProcessValue(value, output, field); // for anything remaining
    }

    void Reset()
    {
        levelSelector = FindAnyObjectByType<UILevelSelector>();
    }
}
