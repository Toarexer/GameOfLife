using GameOfLife.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using GameOfLifeSim;

namespace GameOfLife.Entities;

/// <summary>
/// Represents a rabbit, a type of animal in the simulation.
/// </summary>
public class Rabbit : Animal, ISimulable
{
    private const int NutritionalValue = 3;
    private List<ISimulable> _neighbours = new();
    public MatingPair<Rabbit>? MatingPair;
    private bool _hasMatingPartner;

    /// <summary>
    /// Gets or sets the position of the rabbit on the simulation grid.
    /// </summary>
    public GridPosition Position { get; set; }

    /// <summary>
    /// Gets or sets the next position of the rabbit, used for movement.
    /// </summary>
    public GridPosition? NextPosition { get; set; }

    /// <summary>
    /// Constructor for a Rabbit with an initial position.
    /// Initializes HP and Age properties, and sets the initial position.
    /// </summary>
    /// <param name="position">The initial position of the rabbit.</param>
    public Rabbit(GridPosition position) 
    {
        Hp = 5;
        Age = 0;
        Position = position;
    }

    void ISimulable.Update(Grid grid) 
    {
        _neighbours = grid.SimsInRadius(Position.X, Position.Y, 2).ToList();
        Move(grid);
        if (_neighbours.Any(x => x is Grass))
        {
            Grass grass = (Grass)_neighbours
                .Where(x => x is Grass)
                .OrderByDescending(x => x)
                .First();
            
            Eat(grass);
        }
        IncreaseAge(1);
        Hp--;
    }

    bool ShouldCreateDescendant(Grid grid)
    {
        var rabbits = _neighbours.Where(x => x is Rabbit).ToList();
        if (!HasMatingPartner() || !_neighbours.Any(x => x is Rabbit) && rabbits.Count < 2)
        {
            _hasMatingPartner = false;
            return _hasMatingPartner;
        }

        var rabbit = (Rabbit)rabbits.OrderBy(x => x).First();
        
        MatingPair = new MatingPair<Rabbit>(this, rabbit);
        _hasMatingPartner = true;

        return _hasMatingPartner;
    }

    private bool HasMatingPartner()
    {
        return _hasMatingPartner;
    }

    ISimulable? ISimulable.NewDescendant(Grid grid) {
        if (ShouldCreateDescendant(grid))
            return new Rabbit(Position);
        return null;
    }

    bool ISimulable.ShouldDie() 
    {
        return Hp < 1;
    }

    private bool CanMove() 
    {
        return !_neighbours.Any(x => x is Fox) && !_hasMatingPartner;
    }

    private void Move(Grid grid) {
        
        //Move randomly if hp is full
        if (!ShouldEat() && CanMove()) 
        {
            MoveRandomly(grid);
            return;
        }
        
        //If has mating partner
        if (_hasMatingPartner)
        {
            MoveRandomly(grid);
            MatingPair!.MatingPair2.NextPosition = Position;
            return;
        }
        
        //Move to the next grass to eat else stays, preference by nutritional value
        var grasses = grid.SimsOfTypeInRadius<Grass>(Position.X, Position.Y, 1).OrderByDescending(y => y).ToList();
        if (ShouldEat() && CanMove() && grasses.Count > 0)
        {
            NextPosition = grasses.First().Position;
        }
    }

    private void MoveRandomly(Grid grid) 
    {
        var r = new Random();
        int nextX;
        int nextY;

        do {
            nextX = Position.X + r.Next(-1, 2);
            nextY = Position.Y + r.Next(-1, 2);
        }
        while (!grid.WithinBounds(nextX, nextY));

        NextPosition = new(nextX, nextY);
    }

    private bool ShouldEat() 
    {
        return Hp < 5;
    }
    
    /// <summary>
    /// Attempts to eat a grass object, increasing the rabbit's HP.
    /// </summary>
    /// <param name="grass">The grass object to eat.</param>
    public void Eat(Grass grass) 
    {
        if (ShouldEat() || ((int)grass.Age + Hp <= 5))
        {
            NextPosition = grass.Position;
            Hp += grass.GetEaten();
        } 
    }
    
    /// <summary>
    /// Determines if the rabbit can be eaten by a predator (Fox).
    /// </summary>
    /// <returns>True if the rabbit can be eaten; otherwise, false.</returns>
    public bool CanBeEaten() 
    {
        return _neighbours.Any(x => x is Fox);
    }
    
    /// <summary>
    /// Provides the nutritional value of the rabbit and sets its HP to 0 when eaten.
    /// </summary>
    /// <returns>The nutritional value of the rabbit.</returns>
    public int GetEaten()
    {
        if (!CanBeEaten()) return 0;
        Hp = 0;
        return NutritionalValue;
    }
    
    /// <summary>
    /// Provides display information for the grass, including its type and color.
    /// </summary>
    /// <returns>A `DisplayInfo` object containing type information and a color (gray).</returns>
    public DisplayInfo Info() => new(GetType().FullName ?? GetType().Name, new(150, 150, 100));
}
