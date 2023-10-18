using System;
using System.Collections.Generic;
using System.Linq;
using GameOfLifeLogger;
using GameOfLifeSim;

namespace GameOfLife.Entities
{
    /// <summary>
    /// Represents a type of vegetation, which can be consumed by herbivorous animals.
    /// Implements: ISimulable, IComparable
    /// </summary>
    public class Grass : ISimulable, IComparable<Grass?>
    {
        private int _hp;
        private int _age;
        private int _offsprings;
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
            _age = 0;
            _hp = (int)Age;
            Position = position;
            _offsprings = 0;
        }

        void ISimulable.Update(Grid grid) => Age++;

        /// <summary>
        /// Determines if the grass should die. Currently, it always returns false because it can not die.
        /// </summary>
        /// <returns>False, indicating that the grass should not die under normal conditions.</returns>
        public bool ShouldDie() => false;

        /// <summary>
        /// Determines whether a new descendant (Grass) can be created.
        /// A new descendant is only created when the grass's state is not Seed and conditions are met.
        /// </summary>
        /// <param name="grid">The simulation grid.</param>
        /// <returns>A new descendant (Grass) if the conditions are met; otherwise, null.</returns>
        public ISimulable? NewDescendant(Grid grid)
        {
            if (Age == State.Seed || _offsprings > 1) return null;
            
            Random rnd = new Random();
            List<GridPosition> emptyTiles = new();
            int[] xOffset = { 0, 1, 1,  1,  0, -1, -1, -1 };
            int[] yOffset = { 1, 1, 0, -1, -1, -1,  0,  1 };

            for (int i = 0; i < 8; i++)
            {
                GridPosition nextTile = new GridPosition(Position.X + xOffset[i], Position.Y + yOffset[i]);
    
                if (grid.WithinBounds(nextTile) && grid[nextTile.X, nextTile.Y].Count == 0)
                {
                    emptyTiles.Add(nextTile);
                }
            }
            
            if (emptyTiles.Count == 0) return null;
            _offsprings++;
            
            var grassDescendant = new Grass(emptyTiles.ElementAt(rnd.Next(0, emptyTiles.Count)));
            Logger.Info(grassDescendant.ToString());
            
            return grassDescendant;
        }

        /// <summary>
        /// Gets the nutritional value of the grass and reduces its state.
        /// </summary>
        /// <returns>The nutritional value of the grass, which depends on its state.</returns>
        public int GetEaten()
        {
            _hp = (int)Age;
            if (_hp < 1) return 0;
            Age--;
            return _hp;
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
            if (grass == null) return 0;
            return Age.CompareTo(grass.Age);
        }

        /// <summary>
        /// Provides display information for the grass, including its type and color.
        /// </summary>
        /// <returns>A DisplayInfo object containing type information and a color (green).</returns>
        public DisplayInfo Info() => new DisplayInfo(GetType().FullName ?? GetType().Name, new (0, 255, 0));

        public override string ToString()
        {
            return $"Grass: x: {Position.X} y: {Position.Y}; Age: {(int)Age}; State: {Age} Offsprings: {_offsprings}";
        }
    }
}