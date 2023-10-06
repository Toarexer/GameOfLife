using System;
using System.Collections.Generic;
using System.Xml.Schema;

namespace GameOfLife.GameLogic;

public class Grid {

    readonly List<ISimulable>[,] _cells;

    public int Width => _cells.GetLength(0);
    public int Height => _cells.GetLength(1);

    public Grid(int width, int height) {
        _cells = new List<ISimulable>[width, height];
    }

    public List<ISimulable> this[int x, int y] => _cells[x, y];

    public void CreateSim(ISimulable sim) {
        (int x, int y) = sim.Position();
        this[x, y].Add(sim);
    }

    public bool MoveSim(ISimulable sim, int x, int y) {
        if (x < 0 || x >= Width || y < 0 || y >= Height)
            return false;

        (int ox, int oy) = sim.Position();
        sim.SetPosition(x, y);

        this[ox, oy].Remove(sim);
        this[x, y].Add(sim);

        return true;
    }
    
    public void RemoveSim(ISimulable sim) {
        (int x, int y) = sim.Position();
        this[x, y].Remove(sim);
    }
}
