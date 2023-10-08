using GameOfLife.Entities.Interfaces;
using GameOfLife.GameLogic;

namespace GameOfLife.Entities;

public class Grass : Animal, ICanSpread, ICanBeEaten
{
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
    }
    
    public bool CanBeEaten()
    {
        //If it is in a state of Seed, it can not be eaten
        return GetState() != State.Seed;
    }

    public int GetEaten()
    {
        //Returns 'nutritional' (Hp) value to the animal who eats it depending on the State of the seedling.
        return (int)(State)Hp;
    }

    public State GetState()
    {
        //Returns the state of the seedling
        return (State)Hp;
    }
}