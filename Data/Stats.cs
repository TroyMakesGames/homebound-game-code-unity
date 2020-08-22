namespace NeighbourhoodJam2020.Data
{
    /// <summary>
    /// A collection of the three stats.
    /// </summary>
    public struct Stats
    {
        public int energy;
        public int happiness;
        public int productivity;

        public Stats(int energy, int happiness, int productivity)
        {
            this.energy = energy;
            this.happiness = happiness;
            this.productivity = productivity;
        }
    }
}