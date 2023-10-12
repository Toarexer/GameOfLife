using GameOfLife.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using GameOfLifeSim;

namespace GameOfLife.Entities;

public class Rabbit : Animal, ISimulable, ICanBeEaten {
    
    public readonly int NutritionalValue = 3;
    private List<ISimulable> _neighbours = new();
    public (int x, int y) Position { get; set; } = (0, 0);
    public (int x, int y)? NextPosition { get; set; }
    
    public Rabbit() 
    {
        Hp = 5;
        Age = 0;
    }

    public Rabbit(int posX, int posY) 
    {
        Hp = 5;
        Age = 0;
        Position = (posX, posY);
    }

    void ISimulable.Update(Grid grid) 
    {
        _neighbours = grid.SimsInRadius(Position.x, Position.y, 2).ToList();
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
        return false;
    }
    
    ISimulable ISimulable.NewDescendant(Grid grid) 
    {
        throw new NotImplementedException();
    }
    
    bool ISimulable.ShouldDie() 
    {
        return Hp < 1;
    }

    private bool CanMove() 
    {
        return _neighbours.Count == 0 || !_neighbours.Any(x => x is Fox);
    }

    private void Move(Grid grid) {
        //Move randomly if hp is full
        if (!ShouldEat() && CanMove()) 
        {
            MoveRandomly(grid);
            return;
        }

        //Move to the next grass to eat else stays, preference by nutritional value
        if (ShouldEat() && CanMove() && _neighbours.Any(x => x is Grass)) 
        {
            Position = _neighbours
                .Where(x => x is Grass)
                .OrderByDescending(y => (Grass?)y)
                .FirstOrDefault(x => x is Grass)?.Position ?? Position;
        }
    }

    private void MoveRandomly(Grid grid) 
    {
        var r = new Random();
        int nextX;
        int nextY;

        do {
            nextX = Position.x + r.Next(-1, 2);
            nextY = Position.y + r.Next(-1, 2);
        }
        while (!grid.WithinBounds(nextX, nextY));

        NextPosition = (nextX, nextY);
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
}