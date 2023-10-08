using System.Collections.Generic;
using System.Linq;

namespace GameOfLife.GameLogic;

public class Grid {

    private readonly List<Simulable>[,] _cells;

    public int Width => _cells.GetLength(0);
    public int Height => _cells.GetLength(1);

    internal Grid(int width, int height) {
        _cells = new List<Simulable>[width, height];
    }

    public IReadOnlyList<Simulable> this[int x, int y] => _cells[x, y].AsReadOnly();

    internal bool CreateSim(Simulable sim) {
        (int x, int y) = sim.Position;
        if (x < 0 || x >= Width || y < 0 || y >= Height)
            return false;

        _cells[x, y].Add(sim);
        return true;
    }

    internal bool MoveSim(Simulable sim, int x, int y) {
        if (x < 0 || x >= Width || y < 0 || y >= Height)
            return false;

        (int ox, int oy) = sim.Position;
        sim.Position = (x, y);

        _cells[ox, oy].Remove(sim);
        _cells[x, y].Add(sim);

        return true;
    }

    internal bool RemoveSim(Simulable sim) {
        (int x, int y) = sim.Position;
        if (x < 0 || x >= Width || y < 0 || y >= Height)
            return false;

        _cells[x, y].Remove(sim);
        return true;
    }

    public IEnumerable<T> SimsOfType<T>(int x, int y) where T : Simulable {
        return this[x, y].OfType<T>();
    }
}
