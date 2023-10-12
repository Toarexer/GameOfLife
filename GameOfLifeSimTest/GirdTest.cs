using Microsoft.VisualStudio.TestTools.UnitTesting;
using GameOfLifeSim;

namespace GameOfLifeSimTest;

[TestClass]
public class GirdTest {
    class Dummy : ISimulable {
        public (int x, int y) Position { get; set; }

        public (int x, int y)? NextPosition { get; set; }

        void ISimulable.Update(Grid grid) => throw new System.NotImplementedException();

        bool ISimulable.ShouldDie() => throw new System.NotImplementedException();

        bool ISimulable.ShouldCreateDescendant(Grid grid) => throw new System.NotImplementedException();

        ISimulable ISimulable.NewDescendant(Grid grid) => throw new System.NotImplementedException();
    }

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
            Assert.AreEqual(sim, gm.Grid[sim.Position.x, sim.Position.y].FirstOrDefault());

        Assert.IsNull(gm.Grid[13, 13].FirstOrDefault());
    }

    [TestMethod]
    public void Test01_GridEnumeration() {
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
}
