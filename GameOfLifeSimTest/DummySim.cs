namespace GameOfLifeSimTest;

using GameOfLifeSim;

class Dummy : ISimulable {
    public GridPosition Position { get; set; } = GridPosition.Zero;
    public GridPosition? NextPosition { get; set; }

    public int Health { get; set; } = 100;
    public bool CanMate { get; set; } = false;

    void ISimulable.Update(Grid grid) {
    }

    bool ISimulable.ShouldDie() => Health <= 0;

    ISimulable? ISimulable.NewDescendant(Grid grid) => null;
}
