using System;
using System.Collections.Generic;
using System.Linq;
using GameOfLifeSim;

namespace GameOfLife.Entities;

/// <summary>
/// Represents a rabbit, a type of animal in the simulation.
/// </summary>
public class Rabbit : Animal, ISimulable, IComparable<Rabbit> {
    public MatingPair<Rabbit>? MatingPair;
        
    private const int NutritionalValue = 3;
    private List<Grass> _grasses = new();
    private List<Rabbit> _rabbits = new();
    private List<Fox> _foxes = new();

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

    public override void Update(Grid grid) 
    {
        _grasses = grid.SimsOfTypeInRadius<Grass>(Position.X, Position.Y, 2).OrderDescending().ToList();
        _rabbits = grid.SimsOfTypeInRadius<Rabbit>(Position.X, Position.Y, 2).Except(this).Order().ToList();
        _foxes = grid.SimsOfTypeInRadius<Fox>(Position.X, Position.Y, 2).ToList();
        
        Move(grid);
        Eat();
        
        IncreaseAge(1);
        
    }

    protected override bool ShouldCreateDescendant()
    {
        if (!HasMatingPartner || _rabbits.Count < 1 || Invincibility > 0 || MatingCooldown > 0)
        {
            HasMatingPartner = false;
            return HasMatingPartner;
        }

        var rabbit = _rabbits.OrderBy(x => x).First();

        MatingPair = new MatingPair<Rabbit>(this, rabbit);
        MatingPair.MatingPair1.HasMatingPartner = true;
        //MatingPair.MatingPair2.HasMatingPartner = true;
        
        return HasMatingPartner && _foxes.Count == 0 && _rabbits.Count == 1;
    }

    public override ISimulable? NewDescendant(Grid grid) {
        if (!ShouldCreateDescendant()) return null;

        Console.WriteLine($"Pair1: {MatingPair!.MatingPair1} | Pair2: {MatingPair!.MatingPair2} | New rabbit at: {Position}");
        
        MatingPair!.MatingPair1.HasMatingPartner = false;
        MatingPair!.MatingPair2.HasMatingPartner = false;
        MatingPair = null;
        MatingCooldown = 3;
        
        return new Rabbit(Position);
    }

    public override bool ShouldDie() 
    {
        return Hp < 1;
    }

    private bool CanMove() 
    {
        return !_foxes.Any() && !HasMatingPartner;
    }

    protected override void Move(Grid grid) {
        if (!ShouldEat() || CanMove()) 
        {
            MoveRandomly(grid);
            return;
        }
        
        //If has mating partner
        if (HasMatingPartner)
        {
            MoveRandomly(grid);
            MatingPair!.MatingPair2.NextPosition = Position;
        }
    }

    protected override bool ShouldEat() 
    {
        return Hp < 5;
    }
    
    /// <summary>
    /// Attempts to eat a grass object, increasing the rabbit's HP.
    /// </summary>
    public void Eat() 
    {
        if (_grasses.Count < 1 || HasMatingPartner) return;
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
        return $"Rabbit: x: {Position.X} y: {Position.Y} Hp: {Hp} Invincibility: {Invincibility} HasPair? {HasMatingPartner}";
    }
}
