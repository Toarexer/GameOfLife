using GameOfLife.Entities.Interfaces;

namespace GameOfLife.Entities
{
    /// <summary>
    /// An abstract class for any animal.
    /// Implements: ICanMove, ICanBreed, ICanAge
    /// </summary>
    public abstract class Animal : ICanMove, ICanBreed, ICanAge
    {
        /// <summary>HP (Hit points) will determine if the animal needs to eat or should die.</summary>
        public int Hp { get; protected set; }

        /// <summary>Age determines if the animal can breed or can be eaten.</summary>
        protected int Age { get; set; }

        /// <summary>Returns the Age of the animal.</summary>
        /// <returns>int Age of the animal</returns>
        public int GetAge()
        {
            return Age;
        }

        /// <summary>Increases the Age property of the animal by a specified unit.</summary>
        /// <param name="unit">The amount by which to increase the age.</param>
        public void IncreaseAge(int unit)
        {
            Age += unit;
        }
    }
}