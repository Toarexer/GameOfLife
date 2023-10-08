using Microsoft.VisualStudio.TestTools.UnitTesting;
using GameOfLife.GameLogic;

namespace GameOfLifeTest;

[TestClass]
public class UnitTest {
    [TestMethod]
    public void TestMethod1() {
        GameManager gm = new(32, 32);
    }
}
