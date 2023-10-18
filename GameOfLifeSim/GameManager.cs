using GameOfLifeLogger;

namespace GameOfLifeSim;

public class GameManager {
    public Grid Grid { get; }

    public GameManager(int gridWidth, int gridHeight, int cellCapacity = 8, params ISimulable[] sims) {
        Grid = new(gridWidth, gridHeight, cellCapacity);
        foreach (ISimulable sim in sims)
            try {
                if (!Grid.CreateSim(sim))
                    Logger.Info("Failed to create {0} at {1}x{2}",
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
                foreach (ISimulable sim in Grid[x, y].ToArray())
                    try {
                        if (sim.ShouldDie() && !Grid.RemoveSim(sim, new(x, y)))
                            Logger.Info("Failed to remove {0} at {1}x{2}",
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
                foreach (ISimulable sim in Grid[x, y].ToArray())
                    try {
                        if (sim.NewDescendant(Grid) is not null && !Grid.CreateSim(sim))
                            Logger.Info("Failed to reproduce {0} to {1}x{2}",
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
                foreach (ISimulable sim in Grid[x, y].ToArray()) {
                    try {
                        if (sim.NextPosition is not null && !Grid.MoveSim(sim, new(x, y), sim.NextPosition))
                            Logger.Info("Failed to move {0} from {1}x{2} to {3}x{4}",
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
