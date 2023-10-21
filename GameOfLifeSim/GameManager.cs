using GameOfLifeLogger;

namespace GameOfLifeSim;

public class GameManager {
    /// <summary>The <see cref="GameOfLifeSim.Grid"/> used for the simulation.</summary>
    public Grid Grid { get; }

    /// <summary>Handles the game logic for the simulation.</summary>
    /// <param name="gridWidth">The <see cref="GameOfLifeSim.Grid.Width"/> of the new <see cref="Grid"/>.</param>
    /// <param name="gridHeight">The <see cref="GameOfLifeSim.Grid.Height"/> of the new <see cref="Grid"/>.</param>
    /// <param name="cellCapacity">The <see cref="GameOfLifeSim.Grid.CellCapacity"/> of the new <see cref="Grid"/>.</param>
    /// <param name="sims">An array of <see cref="ISimulable"/> objects to be added to the <see cref="Grid"/></param>
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
                        ISimulable? desc = sim.NewDescendant(Grid);
                        if (desc is not null && !Grid.CreateSim(desc))
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

    /// <summary>
    /// Performs four update loops int the following order:
    /// <list type="number">
    /// <item><description>Run the <see cref="ISimulable.Update"/> method of all Sims.</description></item>
    /// <item><description>Call the <see cref="ISimulable.ShouldDie"/> method of all Sims and remove them if it returns True.</description></item>
    /// <item><description>Call the <see cref="ISimulable.NewDescendant"/> method of all Sims. If it returns an instance, add it to the <see cref="Grid"/>.</description></item>
    /// <item><description>Move all Sims to their <see cref="ISimulable.NextPosition"/> if it is not null. After that set it to null again.</description></item>
    /// </list>
    /// </summary>
    public void Update() {
        UpdateSims();
        KillSims();
        ReproduceSims();
        MoveSims();
    }

    /// <summary>Adds the specified Sims to the <see cref="Grid"/>.</summary>
    /// <param name="sims">The collection of Sims to be added.</param>
    public void AddSims(IEnumerable<ISimulable> sims) {
        foreach (ISimulable sim in sims)
            Grid.CreateSim(sim);
    }

    /// <summary>Adds the specified Sims to the <see cref="Grid"/>.</summary>
    /// <param name="sims">The array of Sims to be added.</param>
    public void AddSims(params ISimulable[] sims) {
        foreach (ISimulable sim in sims)
            Grid.CreateSim(sim);
    }
}
