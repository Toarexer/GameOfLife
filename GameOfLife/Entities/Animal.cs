using GameOfLife.Entities.Interfaces;

namespace GameOfLife.Entities;

public abstract class Animal : ICanMove, ICanBreed, ICanAge {
    public int Hp { get; protected set; }
    protected int Age { get; set; }

    public int GetAge() {
        return Age;
    }

    public void IncreaseAge(int unit) {
        Age += unit;
    }
}
