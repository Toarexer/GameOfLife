using Entities = GameOfLife.Entities;
using Sim = GameOfLifeSim;
using System.Collections.Generic;

namespace GameOfLifeApp;

internal static class Temp {
    public static IEnumerable<Sim.ISimulable> Sims() {
        yield return new Entities.Fox(new Sim.GridPosition(7, 0));
        yield return new Entities.Fox(new Sim.GridPosition(3, 3));
        yield return new Entities.Fox(new Sim.GridPosition(31, 8));
        yield return new Entities.Fox(new Sim.GridPosition(31, 31));

        yield return new Entities.Rabbit(new Sim.GridPosition(3, 3));
        yield return new Entities.Rabbit(new Sim.GridPosition(17, 17));
        yield return new Entities.Rabbit(new Sim.GridPosition(18, 16));

        yield return new Entities.Grass(new Sim.GridPosition(3, 3));
        yield return new Entities.Grass(new Sim.GridPosition(3, 4));
        yield return new Entities.Grass(new Sim.GridPosition(4, 3));
        yield return new Entities.Grass(new Sim.GridPosition(16, 17));
        yield return new Entities.Grass(new Sim.GridPosition(17, 16));
        yield return new Entities.Grass(new Sim.GridPosition(17, 17));
        yield return new Entities.Grass(new Sim.GridPosition(17, 18));
        yield return new Entities.Grass(new Sim.GridPosition(18, 17));
        yield return new Entities.Grass(new Sim.GridPosition(18, 18));

        yield return new Entities.Grass(new Sim.GridPosition(13, 13));
        yield return new Entities.Rabbit(new Sim.GridPosition(13, 13));
        yield return new Entities.Fox(new Sim.GridPosition(13, 13));
    }
}
