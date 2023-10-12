namespace GameOfLife.Entities.Interfaces;

public interface ICanAge
{
    //returns the age
    int GetAge();
    
    //Increases age by a integer unit
    void IncreaseAge(int unit);
}