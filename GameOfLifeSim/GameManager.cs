namespace GameOfLifeSim;

public class GameManager {
    public Grid Grid { get; }

    public GameManager(int gridWidth, int gridHeight, int cellCapacity = 8, params ISimulable[] sims) {
        Grid = new(gridWidth, gridHeight, cellCapacity);
        foreach (ISimulable sim in sims)
            Grid.CreateSim(sim);
    }

    private void UpdateSims() {
        for (int y = 0; y < Grid.Height; y++)
            for (int x = 0; x < Grid.Width; x++)
                foreach (ISimulable sim in Grid[x, y])
                    sim.Update(Grid);
    }

    private void KillSims() {
        for (int y = 0; y < Grid.Height; y++)
            for (int x = 0; x < Grid.Width; x++)
                foreach (ISimulable sim in Grid[x, y].Where(sim => sim.ShouldDie()).ToArray())
                    Grid.RemoveSim(sim);
    }

    private void ReproduceSims() {
        for (int y = 0; y < Grid.Height; y++)
            for (int x = 0; x < Grid.Width; x++)
                foreach (ISimulable sim in Grid[x, y].Select(sim => sim.NewDescendant(Grid)).NotNull().ToArray())
                    Grid.CreateSim(sim);
    }

    private void MoveSims() {
        for (int y = 0; y < Grid.Height; y++)
            for (int x = 0; x < Grid.Width; x++)
                foreach (ISimulable sim in Grid[x, y].Where(sim => sim.NextPosition is not null).ToArray()) {
                    Grid.MoveSim(sim, sim.NextPosition!.X, sim.NextPosition!.Y);
                    sim.NextPosition = null;
                }
    }

    public void Update() {
        UpdateSims();
        KillSims();
        ReproduceSims();
        MoveSims();
    }

    public void AddSims(IEnumerable<ISimulable> sims) {
        foreach (ISimulable sim in sims)
            Grid.CreateSim(sim);
    }

    public void AddSims(params ISimulable[] sims) {
        foreach (ISimulable sim in sims)
            Grid.CreateSim(sim);
    }
}
