using GameOfLife.GameLogic;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameOfLife.Entities;

public class Fox : Animal, ISimulable
{
    private List<Rabbit> _rabbits = new();
    private List<Fox> _foxes = new();
    public (int x, int y) Position { get; set; } = (0, 0);
    public (int x, int y)? NextPosition { get; set; }

    public Fox()
    {
        Hp = 10;
        Age = 0;
    }

    public Fox((int x, int y) position)
    {
        Hp = 10;
        Age = 0;
        Position = position;
    }
    
    void ISimulable.Update(Grid grid)
    {
        _rabbits = new (grid.SimsOfTypeInRadius<Rabbit>(Position.x, Position.y, 2));
        _foxes = new (grid.SimsOfTypeInRadius<Fox>(Position.x, Position.y, 2));
        
        Move(grid);
        if (_rabbits.Count > 0) Eat(_rabbits.First());
        IncreaseAge(1);
        Hp--;
    }
    
    public bool Eat(Rabbit rabbit)
    {
        if (!ShouldEat()) return false;

        int rabbitHpToGive = rabbit.GetEaten();
        if (rabbitHpToGive == 0) return false;

        Hp += rabbitHpToGive;
        NextPosition = rabbit.Position;
        return true;
    }
    
    private bool ShouldEat()
    {
        return Hp <= 7;
    }
    
    bool ISimulable.ShouldDie()
    {
        return Hp < 1;
    }

    bool ISimulable.ShouldCreateDescendant(Grid grid)
    {
        
        return _foxes.Any();
    }

    ISimulable ISimulable.NewDescendant(Grid grid)
    {
        //NextPosition = _foxes.First().Position;
        return new Fox(Position);
    }
    
    private void Move(Grid grid)
    {
        if (!ShouldEat())
        {
            MoveRandomly(grid);
            return;
        }

        if (ShouldEat() && _rabbits.Any())
        {
            NextPosition = _rabbits.First().Position;
        }
    }
    private void MoveRandomly(Grid grid)
    {
        var r = new Random();
        int nextX;
        int nextY;

        do
        {
            nextX = Position.x + r.Next(-1, 2);
            nextY = Position.y + r.Next(-1, 2);
        }
        while (!grid.WithinBounds(nextX, nextY));
        NextPosition = (nextX, nextY);
    }
}