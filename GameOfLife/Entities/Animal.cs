namespace GameOfLife.Entities;

public abstract class Animal
{
    public int HP { get; set; }
    public int[,] Position { get; set; }
}