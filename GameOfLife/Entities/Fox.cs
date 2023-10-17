using System;
using System.Collections.Generic;
using System.Linq;
using GameOfLifeSim;

namespace GameOfLife.Entities;

/// <summary>
/// Represents a fox, a type of animal in the simulation.
/// </summary>
public class Fox : Animal {
    private List<Rabbit> _rabbits = new();
    private List<Fox> _foxes = new();
    
    /// <summary>
    /// Constructor for a Fox with an initial position. Initializes HP and Age properties.
    /// </summary>
    /// <param name="position">The initial position of the fox.</param>
    public Fox(GridPosition position) {
        Hp = 10;
        Age = 0;
        Position = position;
        Invincibility = 3;
        MatingCooldown = 3;
    }

    public override void Update(Grid grid) {
        _rabbits = new List<Rabbit>(grid.SimsOfTypeInRadius<Rabbit>(Position.X, Position.Y, 2));
        _foxes = new List<Fox>(grid.SimsOfTypeInRadius<Fox>(Position.X, Position.Y, 2));

        Move(grid);
        if (_rabbits.Count > 0) Eat(_rabbits.First());
        IncreaseAge(1);
        Hp--;
    }

    /// <summary>
    /// Attempts to eat a rabbit if conditions allow.
    /// </summary>
    /// <param name="rabbit">The rabbit to eat.</param>
    /// <returns>True if the fox successfully eats the rabbit; otherwise, false.</returns>
    public void Eat(Rabbit rabbit) {
        if (!ShouldEat()) return;

        int rabbitHpToGive = rabbit.GetEaten();
        if (rabbitHpToGive == 0) return;

        Hp += rabbitHpToGive;
        NextPosition = rabbit.Position;
    }


    protected override bool ShouldEat() {
        return Hp < 8;
    }

    public override bool ShouldDie() {
        return Hp < 1;
    }

    protected override bool ShouldCreateDescendant() {
        return _foxes.Any();
    }

    public override ISimulable? NewDescendant(Grid grid) {
        if (ShouldCreateDescendant())
            return new Fox(Position);
        return null;
    }

    protected override void Move(Grid grid) {
        if (!ShouldEat()) {
            MoveRandomly(grid);
            return;
        }

        if (ShouldEat() && _rabbits.Any()) {
            NextPosition = _rabbits.First().Position;
        }
    }
    
    /// <summary>
    /// Provides display information for the grass, including its type and color.
    /// </summary>
    /// <returns>A `DisplayInfo` object containing type information and a color (orange).</returns>
    public DisplayInfo Info() => new(GetType().FullName ?? GetType().Name, new(255, 127, 0));
}
