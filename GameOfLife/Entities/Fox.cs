using GameOfLife.GameLogic;

namespace GameOfLife.Entities;

public class Fox : Animal
{
    public Fox()
    {
        Hp = 5;
        Age = 0;
    }

    public Fox(int posX, int posY)
    {
        Hp = 5;
        Age = 0;
    }
}