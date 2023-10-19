using System;
using System.Collections.Generic;
using System.Linq;
using GameOfLifeSim;
using GameOfLifeLogger;

namespace GameOfLife.Entities;

/// <summary>
/// Represents a rabbit, a type of animal in the simulation.
/// </summary>
public class Rabbit : Animal, ISimulable, IComparable<Rabbit> 
{
    public readonly int NutritionalValue = 3;
    private bool _createdDescendant;
    private const int MaxHp = 5;
    private MatingPair<Rabbit>? _matingPair;
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
        Hp = MaxHp;
        Age = 0;
        Position = position;
        Invincibility = 3;
        MatingCooldown = 3;
        _createdDescendant = false;
    }

    public void Update(Grid grid)
    {
        Logger.Info(this.ToString());
        
        _grasses = grid.SimsOfTypeInRadius<Grass>(Position.X, Position.Y, 2).OrderDescending().ToList();
        _rabbits = grid.SimsOfTypeInRadius<Rabbit>(Position.X, Position.Y, 2).Except(this).Order().ToList();
        _foxes = grid.SimsOfTypeInRadius<Fox>(Position.X, Position.Y, 2).ToList();
        
        Move(grid);
        Eat();
        IncreaseAge(1);
        if (_createdDescendant) _createdDescendant = false;
    }

    protected override bool ShouldCreateDescendant()
    {
        if (_createdDescendant || MatingCooldown > 0 || (_matingPair != null && _rabbits.Count > 1)) return false;
        
        var matingPairRabbit = _rabbits.Order().FirstOrDefault();

        if (matingPairRabbit == null) return false;
        _matingPair = new MatingPair<Rabbit>(this, matingPairRabbit)
        {
            MatingPair1 = { HasMatingPartner = true },
            MatingPair2 = { HasMatingPartner = true }
        };

        Logger.Info(_matingPair.ToString());
        return HasMatingPartner && _foxes.Count == 0 && _rabbits.Count == 1;
    }

    public ISimulable? NewDescendant(Grid grid) 
    {
        if (!ShouldCreateDescendant()) return null;
        
        if (_matingPair?.MatingPair1 != null)
        {
            _matingPair.MatingPair1.HasMatingPartner = false;
            _matingPair.MatingPair1.MatingCooldown = 3;
            _matingPair.MatingPair1._matingPair = null;
        }
        
        if (_matingPair?.MatingPair2 != null)
        {
            _matingPair.MatingPair2.HasMatingPartner = false;
            _matingPair.MatingPair2.MatingCooldown = 3;
            _matingPair.MatingPair2._matingPair = null;
        }

        _createdDescendant = true;
        Logger.Info($"New Rabbit at: {Position}");
        return new Rabbit(Position);
    }

    
    public bool ShouldDie() 
    {
        if (Hp < 1)
        {
            Logger.Info($"Died: {this}");
            return true;
        }
        return false;
    }

    private bool CanMove() 
    {
        return !_foxes.Any() && !HasMatingPartner;
    }

    protected override void Move(Grid grid) {
        if (!ShouldEat() || !CanMove()) 
        {
            MoveRandomly(grid);
            return;
        }
        
        if (HasMatingPartner && _matingPair != null)
        {
            MoveRandomly(grid);
            _matingPair!.MatingPair2.NextPosition = Position;
        }
    }

    protected override bool ShouldEat() 
    {
        return Hp < MaxHp;
    }
    
    /// <summary>
    /// Attempts to eat a grass object, increasing the rabbit's HP.
    /// </summary>
    protected override void Eat() 
    {
        if (_grasses.Count < 1 || !ShouldEat()) return;
        Random rnd = new();
            
        Grass[] optimalGrasses = _grasses.Where(x => (int)x.Age + this.Hp == MaxHp).ToArray();
        if (optimalGrasses.Length < 1) return;
        
        Grass grassToEat = optimalGrasses[rnd.Next(0, optimalGrasses.Length)];
        
        NextPosition = grassToEat.Position;
        Hp += grassToEat.GetEaten();
    }
    
    /// <summary>
    /// Determines if the rabbit can be eaten by a predator (Fox).
    /// </summary>
    /// <returns>True if the rabbit can be eaten; otherwise, false.</returns>
    public bool CanBeEaten() => Invincibility == 0 && Hp > 0;
    
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
        return $"Rabbit: {Position}\t|\tHp: {Hp}\t|\tInvincibility: {Invincibility}\t|\tMating CD: {MatingCooldown}  \t|\tHasPair? {HasMatingPartner} \t| CREATED NEW? {_createdDescendant}";
    }
}