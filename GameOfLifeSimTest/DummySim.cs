namespace GameOfLifeSimTest;

using GameOfLifeSim;

class Dummy : ISimulable {
    public (int x, int y) Position { get; set; } = (0, 0);
    public (int x, int y)? NextPosition { get; set; } = null;

    public int Health { get; set; } = 100;

    void ISimulable.Update(Grid grid) { }
    bool ISimulable.ShouldDie() => false;
    bool ISimulable.ShouldCreateDescendant(Grid grid) => false;
    ISimulable ISimulable.NewDescendant(Grid grid) => new Dummy { Position = Position };
}
