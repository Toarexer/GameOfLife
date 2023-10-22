using Microsoft.VisualStudio.TestTools.UnitTesting;
using GameOfLifeSim;

namespace GameOfLifeTests;

[TestClass]
public class GirdTest {
    [TestMethod]
    public void Test01_Grid() {
        DummySim[] dummies = {
            new(new(0, 0)),
            new(new(6, 3)),
            new(new(4, 4)),
            new(new(8, 4)),
            new(new(16, 4)),
        };
        GameManager gm = new(32, 32, 4, dummies);

        foreach (ISimulable sim in dummies)
            Assert.AreSame(sim, gm.Grid[sim.Position.X, sim.Position.Y].FirstOrDefault());

        Assert.IsNull(gm.Grid[13, 13].FirstOrDefault());
    }

    [TestMethod]
    public void Test02_GridEnumeration() {
        DummySim[] dummies = {
            new(new(0, 0)),
            new(new(6, 3)),
            new(new(4, 4)),
            new(new(8, 4)),
            new(new(16, 4)),
        };
        GameManager gm = new(32, 32, 4, dummies);

        foreach (GridPosition pos in dummies.Select(x => x.Position))
            Assert.IsTrue(gm.Grid.Any(x => x.Any(x => x.Position == pos)));
    }

    [TestMethod]
    public void Test03_GridMoveSim() {
        GameManager gm = new(32, 32);
        gm.AddSims(new DummySim(new(12, 12)));

        ISimulable? sim = gm.Grid[12, 12].FirstOrDefault();

        Assert.IsNotNull(sim);

        sim.NextPosition = new(13, 13);

        Assert.AreEqual(new GridPosition(13, 13), sim.NextPosition);

        gm.Update();

        Assert.IsNull(sim.NextPosition);
        Assert.IsNull(gm.Grid[12, 12].FirstOrDefault());
        Assert.IsNotNull(gm.Grid[13, 13].FirstOrDefault());
    }

    [TestMethod]
    public void Test04_GridKillSim() {
        GameManager gm = new(32, 32);
        gm.AddSims(new DummySim(new(13, 13)));

        ISimulable? sim = gm.Grid[13, 13].FirstOrDefault();
        gm.Update();

        Assert.IsNotNull(sim);

        (sim as DummySim)!.Health = 0;
        gm.Update();

        Assert.IsNull(gm.Grid[13, 13].FirstOrDefault());
    }
}
