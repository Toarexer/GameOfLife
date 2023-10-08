using System;
using GameOfLife.Entities.Interfaces;
using GameOfLife.GameLogic;

namespace GameOfLife.Entities;

public class Grass : Simulable, ICanSpread, IComparable<Grass>
{
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