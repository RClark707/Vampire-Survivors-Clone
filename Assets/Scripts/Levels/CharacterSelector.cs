using UnityEngine;

public class CharacterSelector : MonoBehaviour
{
    public static CharacterSelector Instance;
    [HideInInspector]
    public CharacterStatsB characterStats;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public static CharacterStatsB GetCharacterStats()
    {
        if (Instance && Instance.characterStats)
        {
            return Instance.characterStats;
        }
        else
        {
            // no character stats are assigned
            CharacterStatsB[] characters = Resources.FindObjectsOfTypeAll<CharacterStatsB>();
            if (characters.Length > 0)
            {
                return characters[Random.Range(0, characters.Length)];
            }
        }
        Debug.LogError("No Character Stats found anywhere");
        return null;
    }

    public void SelectCharacter(CharacterStatsB character)
    {
        characterStats = character;
    }

    public void DestroySingleton()
    {
        Instance = null;
        Destroy(gameObject);
    }
}
