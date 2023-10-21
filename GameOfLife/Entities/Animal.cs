using System;
using GameOfLifeSim;

namespace GameOfLife.Entities
{
    /// <summary>
    /// An abstract class for any animal.
    /// Implements: ICanMove, ICanBreed, ICanAge
    /// </summary>
    public abstract class Animal
    {
        private int _invincibility;
        private int _matingCooldown;
        
        protected bool HasMatingPartner;
        public GridPosition Position { get; set; }
        public GridPosition? NextPosition { get; set; }
        
        /// <summary>
        /// HP (Hit points) will determine if the animal needs to eat or should die.
        /// </summary>
        public int Hp { get; protected set; }

        /// <summary>
        /// Age determines if the animal can breed or can be eaten.
        /// </summary>
        protected int Age { get; set; }

        protected int Invincibility
        {
            get => _invincibility < 0 ? 0 : _invincibility;
            set => _invincibility = value;
        }

        protected int MatingCooldown
        {
            get => _matingCooldown < 0 ? 0 : _matingCooldown;
            set => _matingCooldown = value;
        }

        /// <summary>
        /// Increases the Age property of the animal by a specified unit.
        /// Decreases the Hp by 1.
        /// Decreases the Invincibility by 1.
        /// Decreases the MatingCooldown by 1.
        /// </summary>
        public void IncreaseAge()
        {
            Age++;
            Hp--;
            Invincibility--;
            MatingCooldown--;
        }

        /// <summary>
        /// Checks if the animal should eat based on its HP.
        /// </summary>
        /// <returns>True if the animal should eat; otherwise, false.</returns>
        protected abstract bool ShouldEat();

        /// <summary>
        /// Attempts to eat an animal if conditions allow.
        /// </summary>
        protected abstract void Eat();
        protected abstract bool ShouldCreateDescendant();
        protected abstract void Move(Grid grid);
        protected void MoveRandomly(Grid grid)
        {
            Random rnd = new();
            int nextX;
            int nextY;

            do {
                nextX = Position!.X + rnd.Next(-1, 2);
                nextY = Position.Y + rnd.Next(-1, 2);
            } while (!grid.WithinBounds(nextX, nextY));

            NextPosition = new(nextX, nextY);
        }
    }
}