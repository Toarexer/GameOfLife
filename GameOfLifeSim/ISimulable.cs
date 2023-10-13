namespace GameOfLifeSim;

public record GridPosition(int X, int Y) {
    public static GridPosition Zero => new(0, 0);
}

public record DisplayInfo(string Name, DisplayInfo.DisplayColor Color) {
    public record DisplayColor(int R, int G, int B) {
        public static DisplayColor Default => new(253, 221, 0);
    }
}

public interface ISimulable {
    /// <summary>The current position of the Sim.</summary>
    GridPosition Position { get; set; }

    /// <summary>The current position where the Sim wants to move in the next turn.</summary>
    /// <summary>If null, the Sim will not move.</summary>
    GridPosition? NextPosition { get; set; }

    /// <summary>Gets called at the beginning of every cycle.</summary>
    void Update(Grid grid);

    /// <summary>Specifies if the Sim should be removed.</summary>
    bool ShouldDie();

    /// <summary>Specifies if a new descendant of the Sim should be created.</summary>
    bool ShouldCreateDescendant(Grid grid);

    /// <summary>Creates a new descendant of the Sim.</summary>
    /// <returns>A new Simulable that will be added into the grid based on it's position.</returns>
    ISimulable NewDescendant(Grid grid);

    DisplayInfo Info() => new(GetType().FullName ?? GetType().Name, DisplayInfo.DisplayColor.Default);
}
