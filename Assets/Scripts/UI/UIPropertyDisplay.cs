using System.Reflection;
using System.Text;
using TMPro;
using UnityEngine;

public abstract class UIPropertyDisplay : MonoBehaviour
{
    public bool updateInEditor = false;
    protected TextMeshProUGUI propertyNames, propertyValues;
    public const string DASH = "-";

    protected virtual void OnEnable() { UpdateFields(); }

    protected virtual void OnDrawGizmosSelected() { if (updateInEditor) UpdateFields(); }

    // Every property display will define its own variables to store the objects it is reading,
    // each class needs to override this function to read the object
    public abstract object GetReadObject();

    // Determine whether to process and show a field or not
    protected virtual bool IsFieldShown(FieldInfo field) { return true; }

    protected virtual StringBuilder ProcessName(string name, StringBuilder output, FieldInfo field)
    {
        if (!IsFieldShown(field)) return output;
        return output.AppendLine(name);
    }

    // By default, this function only processes ints and floats. We can override to process other types, like strings.
    protected virtual StringBuilder ProcessValue(object value, StringBuilder output, FieldInfo field)
    {
        if (!IsFieldShown(field)) return output;

        float fval = value is int ? (int)value : value is float ? (float)value : 0;

        // print as a percentage if it has a range or min attribute assigned and is a float
        PropertyAttribute attribute = (PropertyAttribute)field.GetCustomAttribute<RangeAttribute>() ?? field.GetCustomAttribute<MinAttribute>();
        if (attribute != null && field.FieldType == typeof(float))
        {
            float percentage = Mathf.Round(fval * 100 - 100);

            if (Mathf.Approximately(percentage, 0f))
            {
                output.Append(DASH).Append('\n');
            }
            else
            {
                if (percentage > 0f)
                {
                    output.Append('+');
                }
                output.Append(percentage).Append('%').Append('\n');
            }
        }
        else // we are processing an int
        {
            output.Append(value).Append('\n');
        }

        return output;
    }

    // Returns 2 StringBuilders that will be used to populate two different text boxes in our display classes
    protected virtual StringBuilder[] GetProperties(BindingFlags flags, string targetedType)
    {
        StringBuilder names = new StringBuilder();
        StringBuilder values = new StringBuilder();

        FieldInfo[] fields = System.Type.GetType(targetedType).GetFields(flags);
        foreach (FieldInfo field in fields)
        {
            ProcessName(field.Name, names, field);
            ProcessValue(field.GetValue(GetReadObject()), values, field);
        }

        return new StringBuilder[2] { PrettifyNames(names), values }; // prettify names here?
    }

    // override with how we want to print the stringbuilders ont the text boxes;
    public abstract void UpdateFields();

    public static StringBuilder PrettifyNames(StringBuilder input)
    {
        if (input.Length <= 0) return null;

        StringBuilder result = new StringBuilder();
        char last = '\0';
        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];

            if (last == '\0' || char.IsWhiteSpace(last))
            {
                c = char.ToUpper(c);
            }
            else if (char.IsUpper(c))
            {
                result.Append(' ');
            }
            result.Append(c);

            last = c;
        }

        return result;
    }
}
