using GameOfLife.Entities.Interfaces;
using GameOfLife.GameLogic;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameOfLife.Entities;

public class Fox : Animal
{
    private List<Simulable?> _neighbours = new();

    (int x, int y) _position { get; set; } = (0, 0);
    (int x, int y)? _nextposition { get; set; }

    (int x, int y) ISimulable.Position { get => _position; set => _position = value; }
    (int x, int y)? ISimulable.NextPosition { get => _nextposition; set => _nextposition = value; }
    public Fox()
    {
        Hp = 10;
        Age = 0;
    }

    public Fox(int posX, int posY)
    {
        Hp = 10;
        Age = 0;
    }

    public bool ShouldEat()
    {
        return Hp < 7;
    }

    public void Eat(Rabbit rabbit)
    {
        if (!ShouldEat()) return;
        Hp += rabbit.NutritionalValue;
        rabbit.ShouldDie();
    }
    
    public override void Update(Grid grid)
    {
        _neighbours = GetAllNeighbours(grid);
        Move(grid);
        Eat((Rabbit?)_neighbours.FirstOrDefault(x => x is Rabbit));
        IncreaseAge(1);
        Hp--;
        ShouldDie();
    }

    public override bool ShouldCreateDescendant(Grid grid)
    {
        return base.ShouldCreateDescendant(grid);
    }

    public override bool ShouldDie()
    {
        return Hp < 1;
    }
    private List<Simulable?> GetAllNeighbours(Grid grid)
    {
        var entities = new List<Simulable?>();
        for (int i = -2; i < 3; i++)
            for (int j = -2; j < 3; j++)
        {
                entities.Add(grid.SimsOfType<Grass>(Position.x + i, Position.y + j).FirstOrDefault());
                entities.Add(grid.SimsOfType<Fox>(Position.x + i, Position.y + j).FirstOrDefault());
                entities.Add(grid.SimsOfType<Rabbit>(Position.x + i, Position.y + j).FirstOrDefault());
        }
        return entities;
    }
    private bool CanMove()
    {
        return _neighbours.Count == 0;
    }
    private void Move(Grid grid)
    {
        if (!ShouldEat() && CanMove())
        {
            MoveRandomly(grid);
            return;
        }

        if (ShouldEat() && CanMove() && _neneighbours.Any(x=> x is Rabbit))
        {
            Position = _neneighbours
                .Where(x => x is Rabbit)
                .OrderByDescending(y => (Rabbit?)y)
                .FirstOrDefault(x => x is Rabbit)?.Position ?? Position;
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
        Position = (nextX, nextY);
    }
    private bool ShouldEat()
    {
        return Hp <= 7;
    }
    public void Eat(Rabbit? rabbit)
    {
        if (ShouldEat() && rabbit != null)
        {
            Hp += rabbit.GetEaten();
        }
    }


}