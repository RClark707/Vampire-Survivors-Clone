using UnityEngine;

public class CharacterSelector : MonoBehaviour
{
    public static CharacterSelector Instance;
    [HideInInspector]
    public CharacterStats characterStats;

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

    public static CharacterStats GetCharacterStats()
    {
        return Instance.characterStats;
    }

    public void SelectCharacter(CharacterStats character)
    {
        characterStats = character;
    }

    public void DestroySingleton()
    {
        Instance = null;
        Destroy(gameObject);
    }
}
