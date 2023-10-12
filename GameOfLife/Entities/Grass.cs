using System;
using System.Collections.Generic;
using System.Linq;
using GameOfLife.Entities.Interfaces;
using GameOfLifeSim;

namespace GameOfLife.Entities;

public class Grass : ISimulable, ICanSpread, IComparable<Grass>
{
    //public readonly int NutritionalValue = 2;
    private List<Rabbit> _neighbours = new();

    public (int x, int y) Position { get; set; } = (0, 0);

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
    
    void ISimulable.Update(Grid grid)
    {
        //TODO It should say to rabbits that it could be eaten no matter the distance
        _neighbours = grid.SimsOfTypeInRadius<Rabbit>(Position.x,Position.y, 2).ToList();
        
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