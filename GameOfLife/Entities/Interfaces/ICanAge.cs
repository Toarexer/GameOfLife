namespace GameOfLife.Entities.Interfaces;

public interface ICanAge
{
    //returns the age
    public int GetAge();
    
    //Increases age by a integer unit
    public void IncreaseAge(int unit);
}