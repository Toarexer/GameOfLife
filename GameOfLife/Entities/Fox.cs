using System;
using System.Collections.Generic;
using System.Linq;
using GameOfLifeSim;

namespace GameOfLife.Entities;

/// <summary>
/// Fox class, inherits Animal, implements ISimulable interface.
/// </summary>
public class Fox : Animal {
    private List<Rabbit> _rabbits = new();
    private List<Fox> _foxes = new();

    /// <summary>
    /// Gets or sets the position of the fox as a tuple (x, y).
    /// </summary>
    public GridPosition Position { get; set; }

    /// <summary>
    /// Gets or sets the next position of the fox as an optional tuple (x, y).
    /// </summary>
    public GridPosition? NextPosition { get; set; }
    
    /// <summary>
    /// Constructor for a Fox with an initial position. Initializes HP and Age properties.
    /// </summary>
    /// <param name="position">The initial position of the fox.</param>
    public Fox(GridPosition position) {
        Hp = 10;
        Age = 0;
        Position = position;
    }

    /// <summary>
    /// Updates the state of a simulable object during a simulation step.
    /// This method is called once per simulation step to update the behavior of the object.
    /// </summary>
    /// <param name="grid">The simulation grid in which the object resides.</param>
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

    /// <summary>
    /// Checks if the fox should eat based on its HP.
    /// </summary>
    /// <returns>True if the fox should eat; otherwise, false.</returns>
    protected override bool ShouldEat() {
        return Hp < 8;
    }

    /// <summary>
    /// Determines if the fox should die based on its HP.
    /// </summary>
    /// <returns>True if the fox should die; otherwise, false.</returns>
    public override bool ShouldDie() {
        return Hp < 1;
    }

    /// <summary>
    /// Checks if the fox should create a descendant based on the presence of other foxes.
    /// </summary>
    /// <returns>True if the fox should create a descendant; otherwise, false.</returns>
    protected override bool ShouldCreateDescendant() {
        return _foxes.Any();
    }

    /// <summary>
    /// Creates a new fox as a descendant.
    /// </summary>
    /// <param name="grid">The simulation grid.</param>
    /// <returns>A new instance of the Fox class as a descendant.</returns>
    public override ISimulable? NewDescendant(Grid grid) {
        if (ShouldCreateDescendant())
            return new Fox(Position);
        return null;
    }

    /// <summary>
    /// Moves the fox, either randomly or towards a nearby rabbit if it should eat.
    /// </summary>
    /// <param name="grid">The simulation grid.</param>
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
