using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using GameOfLifeSim;

namespace GameOfLife.Entities;

/// <summary>
/// Represents a rabbit, a type of animal in the simulation.
/// </summary>
public class Rabbit : Animal, ISimulable, IComparable<Rabbit>
{
    private const int NutritionalValue = 3;
    private List<Grass> _grasses = new();
    private List<Rabbit> _rabbits = new();
    private List<Fox> _foxes = new();
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
        Invincibility = 3;
        MatingCooldown = 3;
    }

    void ISimulable.Update(Grid grid) 
    {
        _grasses = grid.SimsOfTypeInRadius<Grass>(Position.X, Position.Y, 2).OrderDescending().ToList();
        _rabbits = grid.SimsOfTypeInRadius<Rabbit>(Position.X, Position.Y, 2).Except(this).Order().ToList();
        _foxes = grid.SimsOfTypeInRadius<Fox>(Position.X, Position.Y, 2).ToList();
        
        Move(grid);
        Eat();
        
        IncreaseAge(1);
        
    }

    bool ShouldCreateDescendant(Grid grid)
    {
        if (!HasMatingPartner() && _rabbits.Count < 1 && Invincibility > 0 && MatingCooldown == 0)
        {
            _hasMatingPartner = false;
            return _hasMatingPartner;
        }

        var rabbit = _rabbits.OrderBy(x => x).First();
        
        MatingPair = new MatingPair<Rabbit>(this, rabbit);
        MatingPair.MatingPair2._hasMatingPartner = true;
        MatingPair.MatingPair1._hasMatingPartner = true;
        
        return _hasMatingPartner && _foxes.Count == 0 && _rabbits.Count == 1;
    }

    private bool HasMatingPartner()
    {
        return _hasMatingPartner;
    }
    
    /// <summary>
    /// Determines whether a new descendant (Rabbit) can be created by the rabbit.
    /// A new descendant is only created when the rabbit has reached a certain age and conditions are met.
    /// </summary>
    /// <param name="grid">The simulation grid.</param>
    /// <returns>A new descendant (Rabbit) if the conditions are met; otherwise, null.</returns>
    ISimulable? ISimulable.NewDescendant(Grid grid) {
        if (!ShouldCreateDescendant(grid)) return null;

        Console.WriteLine($"Pair1: {MatingPair!.MatingPair1} | Pair2: {MatingPair!.MatingPair2} | New rabbit at: {Position}");
        
        MatingPair!.MatingPair1._hasMatingPartner = false;
        MatingPair!.MatingPair2._hasMatingPartner = false;
        MatingPair = null;
        MatingCooldown = 3;
        
        return new Rabbit(Position);
    }

    bool ISimulable.ShouldDie() 
    {
        return Hp < 1;
    }

    private bool CanMove() 
    {
        return !_foxes.Any() && !_hasMatingPartner;
    }

    private void Move(Grid grid) {
        if (!ShouldEat() || CanMove()) 
        {
            MoveRandomly(grid);
            return;
        }
        
        //If has mating partner
        if (_hasMatingPartner)
        {
            MoveRandomly(grid);
            MatingPair!.MatingPair2.NextPosition = Position;
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
    public void Eat() 
    {
        //grasses.Count > 0 && grasses.First().Age != Grass.State.Seed
        if (_grasses.Count < 1 || _hasMatingPartner) return;
        if (ShouldEat())
        {
            Random rnd = new();
            
            Grass[] optimalGrasses = _grasses.Where(x => (int)x.Age + this.Hp == 5).ToArray();
            if (optimalGrasses.Length < 1) return;
            
            Grass grassToEat = optimalGrasses[rnd.Next(0, optimalGrasses.Length)];
            
            NextPosition = grassToEat.Position;
            Hp += grassToEat.GetEaten();
        } 
    }
    
    /// <summary>
    /// Determines if the rabbit can be eaten by a predator (Fox).
    /// </summary>
    /// <returns>True if the rabbit can be eaten; otherwise, false.</returns>
    public bool CanBeEaten() 
    {
        return _foxes.Any() && Invincibility > 0;
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

    public int CompareTo(Rabbit? other)
    {
        return Age.CompareTo(other?.Age);
    }

    public override string ToString()
    {
        return $"Rabbit: x: {Position.X} y: {Position.Y} Hp: {Hp} Invincibility: {Invincibility} HasPair? {_hasMatingPartner}";
    }
}
