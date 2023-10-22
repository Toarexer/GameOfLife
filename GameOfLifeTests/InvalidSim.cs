namespace GameOfLifeTests;

using GameOfLifeSim;

class InvalidSim {
    public GridPosition Position { get; set; }

    public InvalidSim(GridPosition pos) {
        Position = pos;
    }
}
