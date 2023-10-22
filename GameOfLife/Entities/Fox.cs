using System;
using System.Collections.Generic;
using System.Linq;
using GameOfLifeLogger;
using GameOfLifeSim;

namespace GameOfLife.Entities;

/// <summary>
/// Represents a fox, a type of animal in the simulation.
/// </summary>
public class Fox : Animal, ISimulable, IComparable<Fox>
{
    private const int MaxHp = 8;
    private MatingPair<Fox>? _matingPair;
    private List<Rabbit> _rabbits = new();
    private List<Fox> _foxes = new();
    private bool _createdDescendant;

    /// <summary>
    /// Constructor for a Fox with an initial position. Initializes HP and Age properties.
    /// </summary>
    /// <param name="position">The initial position of the fox.</param>
    public Fox(GridPosition position) {
        Hp = MaxHp;
        Age = 0;
        Position = position;
        Invincibility = 3;
        MatingCooldown = 3;
        _createdDescendant = false;
    }

    public void Update(Grid grid) {
        
        _rabbits = grid.SimsOfTypeInRadius<Rabbit>(Position.X, Position.Y, 2).OrderByDescending(x => x).ToList();
        _foxes = grid.SimsOfTypeInRadius<Fox>(Position.X, Position.Y, 2).Except(this).OrderByDescending(x => x).ToList();

        Move(grid);
        Eat();
        IncreaseAge();
        if (_createdDescendant) _createdDescendant = false;
        Logger.Info(this.ToString());
    }
    
    protected override bool ShouldEat() 
    {
        return Hp < MaxHp;
    }
    
    protected override void Eat() {
        if (_rabbits.Count < 1 || HasMatingPartner || !ShouldEat()) return;
        Random rnd = new();
            
        Rabbit[] optimalRabbits = _rabbits.Where(x => x.CanBeEaten()).ToArray();
        if (optimalRabbits.Length < 1) return;
        
        Rabbit rabbitToEat = optimalRabbits[rnd.Next(0, optimalRabbits.Length)];
        
        NextPosition = rabbitToEat.Position;
        int hpToGive = rabbitToEat.GetEaten();
        if (Hp + hpToGive > MaxHp)
        {
            Hp = MaxHp;
            Logger.Info($"{this}\t|\n\tATE: {rabbitToEat}");
            return;
        }
        Hp += hpToGive;
    }

    public bool ShouldDie() 
    {
        if (Hp < 1|| Age > 50)
        {
            Logger.Info($"Died: {this}");
            return true;
        }
        return false;
    }

    protected override bool ShouldCreateDescendant() 
    {
        if (_createdDescendant || MatingCooldown > 0 || (_matingPair != null && _foxes.Count > 1)) return false;
        
        var matingPairFox = _foxes.Order().FirstOrDefault();

        if (matingPairFox == null) return false;
        _matingPair = new MatingPair<Fox>(this, matingPairFox)
        {
            MatingPair1 = { HasMatingPartner = true },
            MatingPair2 = { HasMatingPartner = true }
        };

        Logger.Info(_matingPair.ToString());
        return HasMatingPartner && _foxes.Count == 1;
    }

    public ISimulable? NewDescendant(Grid grid) {
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
        Logger.Info($"New FOX at: {Position}");
        return new Fox(Position);
    }

    protected override void Move(Grid grid) {
        if (HasMatingPartner && _matingPair != null)
        {
            MoveRandomly(grid);
            _matingPair!.MatingPair2.NextPosition = Position;
            return;
        }
        MoveRandomly(grid);
    }
    
    /// <summary>
    /// Provides display information for the fox, including its type and color.
    /// </summary>
    /// <returns>A `DisplayInfo` object containing type information and a color (orange).</returns>
    public DisplayInfo Info() => new(GetType().Name, new(255, 127, 0));

    public int CompareTo(Fox? other)
    {
        return Age.CompareTo(other?.Age);
    }
    
    public override string ToString()
    {
        return $"Fox:\t  {Position}\t|\tAge: {Age}\t|\tHp: {Hp}\t|\tInvincibility: {Invincibility}\t|\tMating CD: {MatingCooldown}  \t|\tHasPair? {HasMatingPartner}  \t| CREATED NEW? {_createdDescendant}";
    }
}