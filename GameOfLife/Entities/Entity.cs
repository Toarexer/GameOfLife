namespace GameOfLife.Entities;

public abstract class Entity
{
    public int Hp { get; protected set; }
    public int Age { get; protected set; }
    public int[,] Position { get; protected set; }
    protected int posX { get; set; }
    protected int posY { get; set; }
}