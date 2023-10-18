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
    }

    public override void Update(Grid grid) {
        _rabbits = grid.SimsOfTypeInRadius<Rabbit>(Position.X, Position.Y, 2).OrderByDescending(x => x).ToList();
        _foxes = grid.SimsOfTypeInRadius<Fox>(Position.X, Position.Y, 2).Except(this).OrderByDescending(x => x).ToList();

        Move(grid);
        Eat();
        IncreaseAge(1);
        Logger.Info(this.ToString());
    }
    
    protected override bool ShouldEat() 
    {
        return Hp < MaxHp;
    }
    
    protected override void Eat() {
        if (_rabbits.Count < 1 || HasMatingPartner || !ShouldEat()) return;
        Random rnd = new();
            
        Rabbit[] optimalRabbits = _rabbits.Where(x => x.CanBeEaten() && x.NutritionalValue + this.Hp == MaxHp).ToArray();
        if (optimalRabbits.Length < 1) return;
        
        Rabbit rabbitToEat = optimalRabbits[rnd.Next(0, optimalRabbits.Length)];
        
        NextPosition = rabbitToEat.Position;
        Hp += rabbitToEat.GetEaten();
    }

    public override bool ShouldDie() 
    {
        if (Hp < 1)
        {
            Logger.Info($"Should Die: {this}");
            return true;
        }
        return false;
    }

    protected override bool ShouldCreateDescendant() 
    {
        if (_matingPair != null && _foxes.Count < 1 && MatingCooldown > 0) return HasMatingPartner;
        
        var matingPairFox = _foxes.Order().FirstOrDefault();

        if (matingPairFox == null) return false;
        _matingPair = new MatingPair<Fox>(this, matingPairFox)
        {
            MatingPair1 = { HasMatingPartner = true }
        };

        Logger.Info(_matingPair.ToString());
        return HasMatingPartner && _foxes.Count == 1;
    }

    public override ISimulable? NewDescendant(Grid grid) {
        if (!ShouldCreateDescendant()) return null;
        
        if (_matingPair?.MatingPair1 != null)
        {
            _matingPair.MatingPair1.HasMatingPartner = false;
        }
        
        if (_matingPair?.MatingPair2 != null)
        {
            _matingPair.MatingPair2.HasMatingPartner = false;
            _matingPair.MatingPair2.MatingCooldown = 2;
        }

        _matingPair = null;
        MatingCooldown = 3;
        return new Fox(Position);
    }

    protected override void Move(Grid grid) {
        if (!ShouldEat()) 
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
    
    /// <summary>
    /// Provides display information for the grass, including its type and color.
    /// </summary>
    /// <returns>A `DisplayInfo` object containing type information and a color (orange).</returns>
    public DisplayInfo Info() => new(GetType().FullName ?? GetType().Name, new(255, 127, 0));

    public int CompareTo(Fox? other)
    {
        return Age.CompareTo(other?.Age);
    }
    
    public override string ToString()
    {
        return $"Fox:\t {Position}\t|\tHp: {Hp}\t|\tInvincibility: {Invincibility}\t|\tMating CD: {MatingCooldown}  \t|\tHasPair? {HasMatingPartner}\t|";
    }
}
