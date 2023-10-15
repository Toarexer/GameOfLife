namespace GameOfLife.Entities.Interfaces
{
    /// <summary>
    /// Interface for entities that can spread or reproduce.
    /// </summary>
    public interface ICanSpread
    {
        /// <summary>
        /// Determines whether the entity has the capability to spread or reproduce.
        /// </summary>
        /// <returns>True if the entity can spread; otherwise, false.</returns>
        bool CanSpread();

        /// <summary>
        /// Represents the action of the entity spreading or reproducing.
        /// </summary>
        void Spread();
    }
}