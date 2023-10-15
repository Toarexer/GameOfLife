namespace GameOfLife.Entities.Interfaces;

/// <summary>
/// Interface for objects that can age and track their age.
/// </summary>
public interface ICanAge
{
    /// <summary>
    /// Gets the current age of the object.
    /// </summary>
    /// <returns>The age of the object as an integer.</returns>
    int GetAge();

    /// <summary>
    /// Increases the age of the object by a specified unit.
    /// </summary>
    /// <param name="unit">The amount by which to increase the age.</param>
    void IncreaseAge(int unit);
}