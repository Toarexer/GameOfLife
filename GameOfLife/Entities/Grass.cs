using System;
using System.Collections.Generic;
using System.Linq;
using GameOfLifeSim;

namespace GameOfLife.Entities
{
    /// <summary>
    /// Represents a type of vegetation, which can be consumed by herbivorous animals.
    /// Implements: ISimulable, ICanSpread, IComparable&lt;Grass&gt;
    /// </summary>
    public class Grass : ISimulable, IComparable<Grass>
    {
        private int Hp { get; set; }
        private int _age;
        public State Age
        {
            get
            {
                if (_age < 1) return State.Seed;
                if (_age < 2) return State.Tender;
                return State.Tuft;
            } 
            private set => _age = (int)value;
        }
        private List<ISimulable> _neighbours = new();

        /// <summary>
        /// Gets or sets the position of the grass on the simulation grid.
        /// </summary>
        public GridPosition Position { get; set; }

        /// <summary>
        /// Gets or sets the next position of the grass, which is used during spreading (not implemented).
        /// </summary>
        public GridPosition? NextPosition { get; set; }
        
        /// <summary>
        /// An enumeration representing the different states of the grass.
        /// </summary>
        public enum State
        {
            Seed = 0,
            Tuft = 1,
            Tender = 2
        }
        
        /// <summary>
        /// Constructor for a Grass object with an initial position.
        /// Initializes HP based on its state and Age, and sets the position.
        /// </summary>
        /// <param name="position">The initial position of the grass.</param>
        public Grass(GridPosition position)
        {
            Hp = (int)Age;
            Position = position;
        }

        void ISimulable.Update(Grid grid)
        {
            // TODO: It should signal to rabbits that it could be eaten no matter the distance.
            _neighbours = grid.SimsInRadius(Position.X, Position.Y, 1).ToList();
            Age++;
        }

        /// <summary>
        /// Determines if the grass should die. Currently, it always returns false.
        /// </summary>
        /// <returns>False, indicating that the grass should not die under normal conditions.</returns>
        public bool ShouldDie()
        {
            return false;
        }

        /// <summary>
        /// Determines if the grass should create descendants. Not implemented.
        /// </summary>
        /// <param name="grid">The simulation grid.</param>
        /// <returns>False, as the creation of descendants is not implemented.</returns>
        public bool ShouldCreateDescendant(Grid grid)
        {
            return Age != State.Seed && _neighbours.Count == 0;
        }

        /// <summary>
        /// Creates a new grass object as a descendant. Not implemented.
        /// </summary>
        /// <param name="grid">The simulation grid.</param>
        /// <returns>Null, as the creation of descendants is not implemented.</returns>
        public ISimulable NewDescendant(Grid grid)
        {
            
            return new Grass(Position);
        }

        /// <summary>
        /// Checks if the grass can be eaten, which depends on its state.
        /// </summary>
        /// <returns>True if the grass can be eaten; otherwise, false.</returns>
        private bool CanBeEaten()
        {
            // If it is in a state of Seed, it cannot be eaten.
            return Age != State.Seed;
        }

        /// <summary>
        /// Gets the nutritional value of the grass and reduces its state.
        /// </summary>
        /// <returns>The nutritional value of the grass, which depends on its state.</returns>
        public int GetEaten()
        {
            Hp = (int)Age;
            if (CanBeEaten())
            {
                Age--;
            }
            return Hp;
        }

        /// <summary>
        /// Gets the current state of the grass.
        /// </summary>
        /// <returns>The state of the grass, represented by the State enum.</returns>

        /// <summary>
        /// Compares this grass object to another based on its HP (state).
        /// </summary>
        /// <param name="grass">The grass object to compare to.</param>
        /// <returns>
        /// -1 if this grass has lower HP, 0 if HP is equal, and 1 if this grass has higher HP.
        /// </returns>
        public int CompareTo(Grass? grass)
        {
            return Hp.CompareTo(grass?.Hp);
        }

        /// <summary>
        /// Provides display information for the grass, including its type and color.
        /// </summary>
        /// <returns>A `DisplayInfo` object containing type information and a color (green).</returns>
        public DisplayInfo Info() => new DisplayInfo(GetType().FullName ?? GetType().Name, new (0, 255, 0));
    }
}