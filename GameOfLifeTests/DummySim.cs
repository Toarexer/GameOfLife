namespace GameOfLifeTests;

using GameOfLifeSim;

class DummySim : ISimulable {
    public GridPosition Position { get; set; }
    public GridPosition? NextPosition { get; set; }

    public int Health { get; set; } = 100;

    public DummySim(GridPosition pos) {
        Position = pos;
    }

    public DummySim() : this(GridPosition.Zero) {
    }

    void ISimulable.Update(Grid grid) {
    }

    bool ISimulable.ShouldDie() => Health <= 0;

    ISimulable? ISimulable.NewDescendant(Grid grid) => null;
}
