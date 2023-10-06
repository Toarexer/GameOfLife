using System;

namespace GameOfLife.Entities;

public abstract class Entity
{
    public int Hp { get; protected set; }
    protected int Age { get; set; }
    public Tuple<int> Position { get; protected set; }
    protected int posX { get; set; }
    protected int posY { get; set; }
}