namespace GameOfLife.Entities.Interfaces
{
    /// <summary>
    /// Interface for entities that can be consumed by predators.
    /// </summary>
    public interface ICanBeEaten
    {
        /// <summary>
        /// Determines whether the entity can be eaten by a predator.
        /// </summary>
        /// <returns>True if the entity can be eaten; otherwise, false.</returns>
        bool CanBeEaten();

        /// <summary>
        /// Represents the act of the entity getting eaten by a predator and contributing to the predator's health.
        /// </summary>
        /// <returns>The nutritional value of the entity being eaten.</returns>
        int GetEaten();
    }
}