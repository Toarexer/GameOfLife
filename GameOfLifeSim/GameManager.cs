using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GameOfLifeSim;

public class GameManager {
    public Grid Grid { get; }

    public GameManager(int gridWidth, int gridHeight, params ISimulable[] sims) {
        Grid = new(gridWidth, gridHeight);
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
                foreach (ISimulable sim in Grid[x, y].Select(sim => sim.NewDescendant(Grid)).NotNull())
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

    public void Update(Action<Exception> exceptionHandler) {
        try {
            Update();
        }
        catch (Exception e) {
            exceptionHandler(e);
        }
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
