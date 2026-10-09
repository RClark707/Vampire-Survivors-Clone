using System.Reflection;
using System.Text;
using TMPro;

public class UIStatsDisplay : UIPropertyDisplay
{
    public Player player;
    public CharacterStatsB character;
    public bool displayCurrentHealth = false;


    public override object GetReadObject()
    {
        if (player) return player.ActualStats;
        else if (character) return character.stats;
        return new CharacterStatsB.Stats();
    }

    // TODO: Make the Stats & Values on a single line!
    public override void UpdateFields()
    {
        if (!player && !character) return;

        StringBuilder[] allStats = GetProperties(BindingFlags.Public | BindingFlags.Instance, "CharacterStatsB+Stats");

        if (!propertyNames) propertyNames = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        if (!propertyValues) propertyValues = transform.GetChild(1).GetComponent<TextMeshProUGUI>();

        if (displayCurrentHealth)
        {
            allStats[0].Insert(0, "Health\n");
            allStats[1].Insert(0, player.Health + "\n");
        }

        if (propertyNames) propertyNames.text = allStats[0].ToString();
        if (propertyValues) propertyValues.text = allStats[1].ToString();

        propertyValues.fontSize = propertyNames.fontSize;
    }

    void Reset()
    {
        player = FindAnyObjectByType<Player>();
    }
}
