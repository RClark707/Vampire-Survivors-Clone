public class Powerup : Passive
{
    [System.Serializable]
    public class Stats
    {
        public string name;
        public int level;
        public Stats(string name, int level)
        {
            this.name = name;
            this.level = level;
        }
    }
}
