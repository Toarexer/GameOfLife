using System.Net.Http;
using GameOfLife.Entities.Interfaces;
using GameOfLife.GameLogic;

namespace GameOfLife.Entities;

public class Rabbit : Animal, ICanBeEaten, ICanMove, ICanBreed, ICanAge
{
    private int nutritionalValue = 3;
    public Rabbit()
    {
        Hp = 5;
        Age = 0;
    }

    public Rabbit(int posX, int posY)
    {
        Hp = 5;
        Age = 0;
    }

    public bool CanBeEaten()
    {
        //If the predator is on current position, it can be eaten
        return false;
    }

    public int GetEaten()
    {
        //Adds HP to the predator, then dies
        return nutritionalValue;
    }

    public int GetAge()
    {
        return Age;
    }

    public void IncreaseAge(int unit)
    {
        Age += unit;
    }
}