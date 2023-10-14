using GameOfLife.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using GameOfLifeSim;

namespace GameOfLife.Entities;

public class Rabbit : Animal, ISimulable, ICanBeEaten {
    
    public readonly int NutritionalValue = 3;
    private List<ISimulable> _neighbours = new();
    public MatingPair<Rabbit> matingPair;
    private bool _hasMatingPartner = false;
    public GridPosition Position { get; set; }
    public GridPosition? NextPosition { get; set; }
    
    public Rabbit() 
    {
        Hp = 5;
        Age = 0;
    }

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

    bool ISimulable.ShouldCreateDescendant(Grid grid)
    {
        var rabbits = _neighbours.Where(x => x is Rabbit).ToList();
        if (!HasMatingPartner() || !_neighbours.Any(x => x is Rabbit) && rabbits.Count < 2)
        {
            _hasMatingPartner = false;
            return _hasMatingPartner;
        }

        var rabbit = (Rabbit)rabbits.OrderBy(x => x).First();
        
        matingPair = new MatingPair<Rabbit>(this, rabbit);
        _hasMatingPartner = true;

        return _hasMatingPartner;
    }

    private bool HasMatingPartner()
    {
        return _hasMatingPartner;
    }

    ISimulable ISimulable.NewDescendant(Grid grid)
    {
        return new Rabbit(Position);
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
            matingPair.MatingPair2.NextPosition = Position;
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
    
    public void Eat(Grass grass) 
    {
        if (ShouldEat() || ((int)grass.GetState() + Hp <= 5))
        {
            NextPosition = grass.Position;
            Hp += grass.GetEaten();
        } 
    }

    public bool CanBeEaten() 
    {
        //If the predator is on current position, it can be eaten
        return _neighbours.Any(x => x is Fox);
    }
    
    public int GetEaten()
    {
        if (!CanBeEaten()) return 0;
        Hp = 0;
        return NutritionalValue;
    }

    public DisplayInfo Info() => new(GetType().FullName ?? GetType().Name, new(150, 150, 100));
}
