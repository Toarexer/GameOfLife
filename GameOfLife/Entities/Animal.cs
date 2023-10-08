using System;
using GameOfLife.Entities.Interfaces;
using GameOfLife.GameLogic;

namespace GameOfLife.Entities;

public abstract class Animal : Simulable, ICanMove, ICanBreed, ICanAge
{
    public int Hp { get; protected set; }
    protected int Age { get; set; }
    
    
    public int GetAge()
    {
        return Age;
    }

    public void IncreaseAge(int unit)
    {
        Age += unit;
    }
    
    public override bool ShouldDie()
    {
        return Hp < 1;
    }
    
    public void Die()
    {
        if (ShouldDie())
        {
            //Destroys entity
        }
    }
}

