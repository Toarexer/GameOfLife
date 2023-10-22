using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GameOfLifeApp;
using GameOfLifeSim;

namespace GameOfLifeTests;

[TestClass]
public class LoadGirdTest {
    [TestMethod]
    public void Test05_LoadGrid() {
        string[] lines = {
            "16x12",
            "Grass,1,1",
            "Grass,2,2",
            "Grass,4,4",
            "Grass,8,8",
            "Rabbit,3,3",
            "Rabbit,6,6",
            "Fox,5,5",
            "Fox,10,10"
        };

        GameManager gm1 = App.LoadGrid(lines);
        Assert.AreEqual(16, gm1.Grid.Width);
        Assert.AreEqual(12, gm1.Grid.Height);
        Assert.AreEqual(8, gm1.Grid.Sum(x => x.Count));

        GameManager gm2 = App.LoadGrid(lines.Select(x => x.Replace(',', ';')));
        Assert.AreEqual(16, gm2.Grid.Width);
        Assert.AreEqual(12, gm2.Grid.Height);
        Assert.AreEqual(8, gm2.Grid.Sum(x => x.Count));
    }

    [TestMethod]
    public void Test06_InvalidInt() {
        string[] lines = {
            "16x12",
            "Grass,WASD,1",
            "Grass,2,2",
            "Grass,4,4",
            "Grass,8",
            "Rabbit,3,3",
            "Rabbit,6,6",
            "Fox,5,5",
            "Fox,10,10"
        };

        Assert.ThrowsException<FormatException>(() => App.LoadGrid(lines));
    }
    
    [TestMethod]
    public void Test07_InvalidLine() {
        string[] lines = {
            "16x12",
            "Grass,1,1",
            "Grass,2,2",
            "Grass,4,4",
            "Grass,8",
            "Rabbit,3,3",
            "Rabbit,6,6",
            "Fox,5,5",
            "Fox,10,10"
        };

        Assert.AreEqual(
            "Error on line 5 at 'Grass,8'",
            Assert.ThrowsException<App.InvalidLineException>(() => App.LoadGrid(lines)).Message
        );
    }
    
    [TestMethod]
    public void Test08_MissingSim() {
        string[] lines = {
            "16x12",
            "Grass,1,1",
            "Grass,2,2",
            "Wolf,4,4",
            "Grass,8,8",
            "Rabbit,3,3",
            "Rabbit,6,6",
            "Fox,5,5",
            "Fox,10,10"
        };

        Assert.ThrowsException<ReflectionTypeLoadException>(() => App.LoadGrid(lines));
    }
    
    [TestMethod]
    public void Test09_NotSim() {
        string[] lines = {
            "1x1",
            $"{typeof(InvalidSim).FullName},0,0"
        };

        Assert.ThrowsException<TypeLoadException>(() => App.LoadGrid(lines, null, true));
    }
}
