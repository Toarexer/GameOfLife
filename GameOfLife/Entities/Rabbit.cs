using System.Net.Http;
using GameOfLife.Entities.Interfaces;

namespace GameOfLife.Entities;

public class Rabbit : Animal, ICanBeEaten, ICanMove, ICanBreed, ICanAge
{
    private int nutritionalValue = 3;
    public Rabbit()
    {
        Hp = 5;
        Age = 0;
        posX = 0;
        posY = 0;
    }

    public Rabbit(int posX, int posY)
    {
        Hp = 5;
        Age = 0;
        this.posX = posX;
        this.posY = posY;
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