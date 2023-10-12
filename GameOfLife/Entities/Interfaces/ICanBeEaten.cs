using System.IO.Enumeration;

namespace GameOfLife.Entities.Interfaces;

public interface ICanBeEaten
{
    //Returns true if the entity can be eaten
    bool CanBeEaten();
    
    //Entity gets eaten and adds HP to the predator
    int GetEaten();
}