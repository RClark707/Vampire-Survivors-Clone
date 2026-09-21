using UnityEngine;

public abstract class ItemStatsB : ScriptableObject
{
    public Sprite icon;
    public int maxLevel;

    [System.Serializable]
    public struct Evolution
    {
        public string name;
        public enum EvolutionRequirements { Automatic, TreasureChest }
        public EvolutionRequirements evoReq;

        [System.Flags] public enum Consumption { Passives = 1, Weapons = 2 }
        public Consumption consumes;

        public int evolutionLevel;
        public Config[] catalysts;
        public Config outcome;

        [System.Serializable]
        public struct Config
        {
            public ItemStatsB itemType;
            public int level;
        }
    }

    public Evolution[] evolutionData; // this allows the item to evolve multiple times!
}
