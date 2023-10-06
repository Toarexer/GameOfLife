using System.Net.Http;
using GameOfLife.Entities.Interfaces;

namespace GameOfLife.Entities;

public class Rabbit : Animal, ICanBeEaten
{
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
}