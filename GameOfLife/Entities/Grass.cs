using GameOfLife.Entities.Interfaces;

namespace GameOfLife.Entities;

public class Grass : Entity, ICanSpread, ICanBeEaten
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
        posX = 0;
        posY = 0;
    }
    
    public Grass(int posX, int posY)
    {
        Hp = (int)State.Seed;
        Age = 0;
        this.posX = posX;
        this.posY = posY;
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