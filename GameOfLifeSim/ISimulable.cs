namespace GameOfLifeSim;

/// <summary>Represents a position within a <see cref="Grid"/>.</summary>
/// <param name="X">The horizontal position within the <see cref="Grid"/> that starts from 0.</param>
/// <param name="Y">The vertical position within the <see cref="Grid"/> that starts from 0.</param>
public record GridPosition(int X, int Y) {
    public static GridPosition Zero => new(0, 0);
}

/// <summary>Information that defines how the Sim should be displayed.</summary>
/// <param name="Name">The name of Sim.</param>
/// <param name="Color">The <see cref="DisplayColor"/> that defines the RBB color of the Sim.</param>
public record DisplayInfo(string Name, DisplayInfo.DisplayColor Color) {
    /// <summary>Represents an RBG color.</summary>
    /// <param name="R">Value of the red color from 0 to 255.</param>
    /// <param name="G">Value of the green color from 0 to 255.</param>
    /// <param name="B">Value of the blue color from 0 to 255.</param>
    public record DisplayColor(int R, int G, int B) {
        public static DisplayColor Default => new(253, 221, 0);
    }
}

/// <summary>Declares the necessary members for a Sim that can be simulated by the <see cref="GameManager"/>.</summary>
public interface ISimulable {
    /// <summary>The current position of the Sim. Setting in manually has no effect and is only for keeping track of the Sim's own position.</summary>
    /// <remarks>The only exception is when the Sim is being added to a <see cref="Grid"/> and no position is provided. I that case the current <see cref="Position"/> of the Sim is used.</remarks>
    GridPosition Position { get; set; }

    /// <summary>The position where the Sim wants to move in the next turn. If null, the Sim will not move.</summary>
    GridPosition? NextPosition { get; set; }

    /// <summary>Gets called at the beginning of every simulation cycle.</summary>
    void Update(Grid grid);

    /// <summary>Specifies if the Sim should be removed.</summary>
    bool ShouldDie();

    /// <summary>Creates a new descendant of the Sim.</summary>
    /// <returns>A new <see cref="ISimulable"/> that will be added into the grid based on it's <see cref="Position"/>.</returns>
    ISimulable? NewDescendant(Grid grid);

    /// <returns>A <see cref="DisplayInfo"/> that specifies the name and the <see cref="DisplayInfo.DisplayColor"/> the Sim should be displayed with.</returns>
    DisplayInfo Info() => new(GetType().FullName ?? GetType().Name, DisplayInfo.DisplayColor.Default);
}
