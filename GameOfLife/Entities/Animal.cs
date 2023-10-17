using System;
using System.Collections.Generic;
using GameOfLifeSim;

namespace GameOfLife.Entities
{
    /// <summary>
    /// An abstract class for any animal.
    /// Implements: ICanMove, ICanBreed, ICanAge
    /// </summary>
    public abstract class Animal : ISimulable
    {
        private int _invincibility;
        private int _matingCooldown;
        
        protected bool HasMatingPartner;
        public MatingPair<Rabbit>? MatingPair;
        public GridPosition Position { get; set; }
        public GridPosition? NextPosition { get; set; }
        
        /// <summary>HP (Hit points) will determine if the animal needs to eat or should die.</summary>
        public int Hp { get; protected set; }

        /// <summary>Age determines if the animal can breed or can be eaten.</summary>
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
        /// <param name="unit">The amount by which to increase the age.</param>
        public void IncreaseAge(int unit)
        {
            Age += unit;
            //Hp--;
            Invincibility--;
            MatingCooldown--;
        }

        protected abstract bool ShouldEat();
        protected abstract bool ShouldCreateDescendant();
        protected abstract void Move(Grid grid);

        protected void MoveRandomly(Grid grid)
        {
            Random rnd = new();
            int nextX;
            int nextY;

            do {
                nextX = Position.X + rnd.Next(-1, 2);
                nextY = Position.Y + rnd.Next(-1, 2);
            } while (!grid.WithinBounds(nextX, nextY));

            NextPosition = new(nextX, nextY);
        }
        
        public abstract void Update(Grid grid);
        public abstract bool ShouldDie();
        public abstract ISimulable? NewDescendant(Grid grid);
    }
}