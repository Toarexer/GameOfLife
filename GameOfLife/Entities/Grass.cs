using System;
using GameOfLife.Entities.Interfaces;
using GameOfLife.GameLogic;

namespace GameOfLife.Entities;

public class Grass : ISimulable, ICanSpread, IComparable<Grass>
{
    //public readonly int NutritionalValue = 2;
    private List<ISimulable?> _neighbours = new();

    (int x, int y) _position { get; set; } = (0, 0);
    (int x, int y)? _nextposition { get; set; }

    (int x, int y) ISimulable.Position { get => _position; set => _position = value; }
    (int x, int y)? ISimulable.NextPosition { get => _nextposition; set => _nextposition = value; }

    private int Hp { get; set; }
    private int Age { get; set; }
    public enum State
    {
        Seed = 0,
        Tuft = 1,
        Tender = 2
    }
    
    
    public Grass()
    {
        Hp = (int)State.Seed;
        Age = 0;
    }
    
    public Grass(int posX, int posY)
    {
        Hp = (int)State.Seed;
        Age = 0;
        Position = (posX, posY);
    }
    
    public override void Update(Grid grid)
    {
        _neighbours = GetAllNeighbours(grid);   
    }
    private List<ISimulable?> GetAllNeighbours(Grid grid)
    {
        var entities = new List<ISimulable?>();

        for (int i = -2; i < 3; i++)
            for (int j = -2; j < 3; j++)
            {
                entities.Add(grid.SimsOfType<Grass>(_position.x + i, _position.y + j).FirstOrDefault());
                entities.Add(grid.SimsOfType<Fox>(_position.x + i, _position.y + j).FirstOrDefault());
                entities.Add(grid.SimsOfType<Rabbit>(_position.x + i, _position.y + j).FirstOrDefault());
            }

        return entities;
    }

    public bool CanBeEaten()
    {
        //If it is in a state of Seed, it can not be eaten
        return GetState() != State.Seed;
    }

    public int GetEaten()
    {
        int tempHp = (int)GetState();
        if (CanBeEaten())
        {
            Hp = (int)GetState() - 1;
        }

        return tempHp;
    }

    public State GetState()
    {
        //Returns the state of the seedling
        return (State)Hp;
    }

    public int CompareTo(Grass? grass)
    {
        return Hp.CompareTo(grass?.Hp);
    }

    

    public bool CanSpread()
    {
        throw new NotImplementedException();
    }

    public void Spread()
    {
        throw new NotImplementedException();
    }
}