using Microsoft.VisualStudio.TestTools.UnitTesting;
using GameOfLifeSim;

namespace GameOfLifeSimTest;

[TestClass]
public class GirdTest {
    [TestMethod]
    public void Test01_Grid() {
        Dummy[] dummies = {
            new() { Position = (0, 0) },
            new() { Position = (6, 3) },
            new() { Position = (4, 4) },
            new() { Position = (8, 4) },
            new() { Position = (16, 4) },
        };
        GameManager gm = new(32, 32, dummies);

        foreach (ISimulable sim in dummies)
            Assert.AreSame(sim, gm.Grid[sim.Position.x, sim.Position.y].FirstOrDefault());

        Assert.IsNull(gm.Grid[13, 13].FirstOrDefault());
    }

    [TestMethod]
    public void Test02_GridEnumeration() {
        Dummy[] dummies = {
            new() { Position = (0, 0) },
            new() { Position = (6, 3) },
            new() { Position = (4, 4) },
            new() { Position = (8, 4) },
            new() { Position = (16, 4) },
        };
        GameManager gm = new(32, 32, dummies);

        foreach ((int x, int y) pos in dummies.Select(x => x.Position))
            Assert.IsTrue(gm.Grid.Any(x => x.Any(x => x.Position == (pos.x, pos.y))));
    }

    [TestMethod]
    public void Test03_GridMoveSim() {
        GameManager gm = new(32, 32);
        gm.AddSims(new Dummy { Position = (12, 12) });

        ISimulable? sim = gm.Grid[12, 12].FirstOrDefault();

        Assert.IsNotNull(sim);

        sim.NextPosition = (13, 13);

        Assert.AreEqual((13, 13), sim.NextPosition);

        gm.Update();

        Assert.IsNull(sim.NextPosition);
        Assert.IsNull(gm.Grid[12, 12].FirstOrDefault());
        Assert.IsNotNull(gm.Grid[13, 13].FirstOrDefault());
    }
}
