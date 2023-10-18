using GameOfLifeLogger;

namespace GameOfLifeSim;

public class GameManager {
    public Grid Grid { get; }

    public GameManager(int gridWidth, int gridHeight, int cellCapacity = 8, params ISimulable[] sims) {
        Grid = new(gridWidth, gridHeight, cellCapacity);
        foreach (ISimulable sim in sims)
            try {
                if (!Grid.CreateSim(sim))
                    Logger.Info("Failed to create {} at {}x{}",
                        sim.GetType().FullName,
                        sim.Position.X,
                        sim.Position.Y
                    );
            }
            catch (Exception e) {
                Logger.Error(e.ToString());
            }
    }

    private void UpdateSims() {
        for (int y = 0; y < Grid.Height; y++)
            for (int x = 0; x < Grid.Width; x++)
                foreach (ISimulable sim in Grid[x, y])
                    try {
                        sim.Update(Grid);
                    }
                    catch (Exception e) {
                        Logger.Error(e.ToString());
                    }
    }

    private void KillSims() {
        for (int y = 0; y < Grid.Height; y++)
            for (int x = 0; x < Grid.Width; x++)
                foreach (ISimulable sim in Grid[x, y].Where(sim => sim.ShouldDie()).ToArray())
                    try {
                        if (!Grid.RemoveSim(sim))
                            Logger.Info("Failed to remove {} at {}x{}",
                                sim.GetType().FullName,
                                sim.Position.X,
                                sim.Position.Y
                            );
                    }
                    catch (Exception e) {
                        Logger.Error(e.ToString());
                    }
    }

    private void ReproduceSims() {
        for (int y = 0; y < Grid.Height; y++)
            for (int x = 0; x < Grid.Width; x++)
                foreach (ISimulable sim in Grid[x, y].Select(sim => sim.NewDescendant(Grid)).NotNull().ToArray())
                    try {
                        if (!Grid.CreateSim(sim))
                            Logger.Info("Failed to reproduce {} to {}x{}",
                                sim.GetType().FullName,
                                sim.Position.X,
                                sim.Position.Y
                            );
                    }
                    catch (Exception e) {
                        Logger.Error(e.ToString());
                    }
    }

    private void MoveSims() {
        for (int y = 0; y < Grid.Height; y++)
            for (int x = 0; x < Grid.Width; x++)
                foreach (ISimulable sim in Grid[x, y].Where(sim => sim.NextPosition is not null).ToArray()) {
                    try {
                        if (!Grid.MoveSim(sim, sim.NextPosition!.X, sim.NextPosition!.Y))
                            Logger.Info("Failed to move {} from {}x{} to {}x{}",
                                sim.GetType().FullName,
                                sim.NextPosition!.X,
                                sim.NextPosition!.Y,
                                sim.Position.X,
                                sim.Position.Y
                            );
                    }
                    catch (Exception e) {
                        Logger.Error(e.ToString());
                    }

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
