using System;
using System.Collections.Generic;
using System.Linq;
using GameOfLife.Entities.Interfaces;
using GameOfLife.GameLogic;

namespace GameOfLife.Entities;

public class Rabbit : Animal, ICanBeEaten
{
    public readonly int NutritionalValue = 3;
    private List<Simulable?> _neighbours = new List<Simulable?>(); 
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

    public override void Update(Grid grid)
    {
        _neighbours = GetAllNeighbours(grid);
        Move(grid);
        Eat((Grass?)_neighbours.FirstOrDefault(x => x is Grass));
        IncreaseAge(1);
        Hp--;
        Die();
    }

    public override bool ShouldCreateDescendant(Grid grid)
    {
        return false;
    }

    public override bool ShouldDie()
    {
        return Hp < 1 || CanBeEaten();
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
        return _neighbours.Count == 0 || !_neighbours.Any(x => x is Fox);
    }

    private void Move(Grid grid)
    {
        //Move randomly if hp is full
        if (!ShouldEat() && CanMove())
        {
            MoveRandomly(grid);
            return;
        }

        //Move to the next grass to eat else stays, preference by nutritional value
        if (ShouldEat() && CanMove() && _neighbours.Any(x=> x is Grass))
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
        return Hp < 5;
    }

    public void Eat(Grass? grass)
    {
        if (ShouldEat() && grass != null)
        {
            Hp += grass.GetEaten();
        }
    }

    public bool CanBeEaten()
    {
        //If the predator is on current position, it can be eaten
        return _neighbours.Any(x=> x is Fox);
    }

    public void GetEaten()
    {
        if (!CanBeEaten()) return;
        var fox = (Fox)_neighbours.FirstOrDefault(x => x is Fox)!;
        fox.Eat(this);
    }
}