using System;

namespace GameOfLife.GameLogic; 

public abstract class Simulable {
    public (int x, int y) Position { get; set; } = (0, 0);
    public (int x, int y)? NextPosition { get; protected set; } = null;
    public virtual void Update(Grid grid) => throw new NotImplementedException();
    public virtual bool ShouldCreateDescendant(Grid grid) => false;
    public virtual bool ShouldDie() => throw new NotImplementedException();
    public virtual Simulable NewDescendant() => throw new NotImplementedException();
}
