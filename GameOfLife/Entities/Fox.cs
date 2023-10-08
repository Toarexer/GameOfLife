using GameOfLife.Entities.Interfaces;
using GameOfLife.GameLogic;

namespace GameOfLife.Entities;

public class Fox : Animal
{
    public Fox()
    {
        Hp = 10;
        Age = 0;
    }

    public Fox(int posX, int posY)
    {
        Hp = 10;
        Age = 0;
    }

    public bool ShouldEat()
    {
        return Hp < 7;
    }

    public void Eat(Rabbit rabbit)
    {
        if (!ShouldEat()) return;
        Hp += rabbit.NutritionalValue;
        rabbit.Die();
    }
    
    public override void Update(Grid grid)
    {
        //Move();
        //Eat();
        

        IncreaseAge(1);
        Hp--;
        Die();
    }

    public override bool ShouldCreateDescendant(Grid grid)
    {
        return base.ShouldCreateDescendant(grid);
    }

    public override bool ShouldDie()
    {
        return Hp < 1;
    }
    
    
}