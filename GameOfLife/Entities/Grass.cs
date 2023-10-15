using System;
using System.Collections.Generic;
using System.Linq;
using GameOfLifeSim;

namespace GameOfLife.Entities
{
    /// <summary>
    /// Represents a type of vegetation, which can be consumed by herbivorous animals.
    /// Implements: ISimulable, IComparable
    /// </summary>
    public class Grass : ISimulable, IComparable<Grass>
    {
        public int Hp { get; set; }
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
        /// Determines whether a new descendant (Grass) can be created.
        /// A new descendant is only created when the grass's state is not Seed and conditions are met.
        /// </summary>
        /// <param name="grid">The simulation grid.</param>
        /// <returns>A new descendant (Grass) if the conditions are met; otherwise, null.</returns>
        public ISimulable? NewDescendant(Grid grid)
        {
            if (Age == State.Seed)
            {
                return null;
            }
            
            var top = new GridPosition(Position.X, Position.Y + 1);
            var right = new GridPosition(Position.X + 1, Position.Y);
            var bottom = new GridPosition(Position.X, Position.Y - 1);
            var left = new GridPosition(Position.X - 1, Position.Y - 1);
            
            if (grid.WithinBounds(top) && grid[top.X, top.Y].Count == 0)
            {
                return new Grass(top);
            }

            if (grid.WithinBounds(right) && grid[right.X, right.Y].Count == 0)
            {
                return new Grass(right);
            }
            
            if (grid.WithinBounds(bottom) && grid[bottom.X, bottom.Y].Count == 0)
            {
                return new Grass(bottom);
            }
            
            if (grid.WithinBounds(left) && grid[left.X, left.Y].Count == 0)
            {
                return new Grass(left);
            }

            return null;
        }

        /// <summary>
        /// Checks if the grass can be eaten, which depends on its state.
        /// </summary>
        /// <returns>True if the grass can be eaten; otherwise, false.</returns>
        public bool CanBeEaten()
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
        /// Compares this grass object to another based on its Age (state).
        /// </summary>
        /// <param name="grass">The grass object to compare to.</param>
        /// <returns>
        /// -1 if this grass has a lower Age, 0 if Ages are equal, and 1 if this grass has a higher Age.
        /// </returns>
        public int CompareTo(Grass? grass)
        {
            return Age.CompareTo(grass?.Age);
        }

        /// <summary>
        /// Provides display information for the grass, including its type and color.
        /// </summary>
        /// <returns>A DisplayInfo object containing type information and a color (green).</returns>
        public DisplayInfo Info() => new DisplayInfo(GetType().FullName ?? GetType().Name, new (0, 255, 0));

        public override string ToString()
        {
            return $"Grass: x: {Position.X} y: {Position.Y}; Age: {(int)Age}; State: {Age}";
        }
    }
}