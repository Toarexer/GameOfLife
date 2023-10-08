using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GameOfLife.GameLogic;

public class Grid : IEnumerable<IReadOnlyList<Simulable>> {
    public class GridEnumerator : IEnumerator<IReadOnlyList<Simulable>> {
        private readonly Grid _grid;
        private int _index = -1;

        public GridEnumerator(Grid grid) => _grid = grid;

        public bool MoveNext() => ++_index < _grid.Width * _grid.Height;

        public void Reset() => _index = -1;

        public IReadOnlyList<Simulable> Current => _grid[_index % _grid.Width, _index / _grid.Width];

        object IEnumerator.Current => Current;

        public void Dispose() {
        }
    }

    private readonly List<Simulable>[,] _cells;

    public int Width => _cells.GetLength(0);
    public int Height => _cells.GetLength(1);

    internal Grid(int width, int height) {
        _cells = new List<Simulable>[width, height];
    }

    public IEnumerator<IReadOnlyList<Simulable>> GetEnumerator() => new GridEnumerator(this);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    
    public bool WithinBounds(int x, int y) => x >= 0 && x < Width && y >= 0 && y < Height;

    public IReadOnlyList<Simulable> this[int x, int y] {
        get {
            if (WithinBounds(x, y))
                return _cells[x, y].AsReadOnly();
            return new List<Simulable>().AsReadOnly();
        }
    }

    public IEnumerable<T> SimsOfType<T>(int x, int y) where T : Simulable {
        return this[x, y].OfType<T>();
    }

    internal bool CreateSim(Simulable sim) {
        (int x, int y) = sim.Position;
        if (!WithinBounds(x, y))
            return false;

        _cells[x, y].Add(sim);
        return true;
    }

    internal bool MoveSim(Simulable sim, int x, int y) {
        if (!WithinBounds(x, y))
            return false;

        (int ox, int oy) = sim.Position;
        sim.Position = (x, y);

        _cells[ox, oy].Remove(sim);
        _cells[x, y].Add(sim);

        return true;
    }

    internal bool RemoveSim(Simulable sim) {
        (int x, int y) = sim.Position;
        if (!WithinBounds(x, y))
            return false;

        _cells[x, y].Remove(sim);
        return true;
    }
}
