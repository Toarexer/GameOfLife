using System;
using GameOfLife.Entities.Interfaces;
using GameOfLife.GameLogic;

namespace GameOfLife.Entities;

public abstract class Animal : Simulable, ICanMove, ICanBreed
{
    public int Hp { get; protected set; }
    protected int Age { get; set; }
    public void Die()
    {
        if (Hp < 0)
        {
            //Destroys entity
        }
    }
}

