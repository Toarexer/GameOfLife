using System;

namespace GameOfLife.GameLogic;

public abstract class Simulable {
    /// <summary>The current position of the Sim.</summary>
    public (int x, int y) Position { get; internal set; } = (0, 0);

    /// <summary>The current position where the Sim wants to move in the next turn.</summary>
    /// <summary>If null, the Sim will not move.</summary>
    public (int x, int y)? NextPosition { get; protected internal set; }

    /// <summary>Gets called at the beginning of every cycle.</summary>
    public virtual void Update(Grid grid) => throw new NotImplementedException();

    /// <summary>Specifies if the Sim should be removed.</summary>
    public virtual bool ShouldDie() => throw new NotImplementedException();

    /// <summary>Specifies if a new descendant of the Sim should be created.</summary>
    public virtual bool ShouldCreateDescendant(Grid grid) => false;

    /// <summary>Creates a new descendant of the Sim.</summary>
    /// <returns>A new Simulable that will be created at the position of it's parent Sim.</returns>
    public virtual Simulable NewDescendant() => throw new NotImplementedException();
}
