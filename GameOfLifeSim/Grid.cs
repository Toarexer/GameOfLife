using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GameOfLifeSim;

public class Grid : IEnumerable<IReadOnlyList<ISimulable>> {
    public class GridEnumerator : IEnumerator<IReadOnlyList<ISimulable>> {
        private readonly Grid _grid;
        private int _index = -1;

        public GridEnumerator(Grid grid) => _grid = grid;

        public bool MoveNext() => ++_index < _grid.Width * _grid.Height;

        public void Reset() => _index = -1;

        public IReadOnlyList<ISimulable> Current => _grid[_index % _grid.Width, _index / _grid.Width];

        object IEnumerator.Current => Current;

        public void Dispose() {
        }
    }

    private readonly List<ISimulable>[,] _cells;

    public int Width => _cells.GetLength(0);
    public int Height => _cells.GetLength(1);

    internal Grid(int width, int height) {
        _cells = new List<ISimulable>[width, height];
        for (int y = 0; y < Height; y++)
            for (int x = 0; x < Width; x++)
                _cells[x, y] = new();
    }

    public IEnumerator<IReadOnlyList<ISimulable>> GetEnumerator() => new GridEnumerator(this);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public bool WithinBounds(int x, int y) => x >= 0 && x < Width && y >= 0 && y < Height;

    public IReadOnlyList<ISimulable> this[int x, int y] {
        get {
            if (WithinBounds(x, y))
                return _cells[x, y].AsReadOnly();
            return new List<ISimulable>().AsReadOnly();
        }
    }

    public IEnumerable<T> SimsOfType<T>(int x, int y) where T : ISimulable {
        return this[x, y].OfType<T>();
    }

    public IEnumerable<T> SimsOfTypeInRadius<T>(int ox, int oy, int r) where T : ISimulable {
        for (int y = oy - r; y < oy + r; y++)
            for (int x = ox - r; x < ox + r; x++)
                foreach (T sim in this[x, y].OfType<T>())
                    yield return sim;
    }

    public IEnumerable<ISimulable> SimsInRadius(int ox, int oy, int r) {
        for (int y = oy - r; y < oy + r; y++)
            for (int x = ox - r; x < ox + r; x++)
                foreach (ISimulable sim in this[x, y])
                    yield return sim;
    }

    internal bool CreateSim(ISimulable sim) {
        (int x, int y) = sim.Position;
        if (!WithinBounds(x, y))
            return false;

        _cells[x, y].Add(sim);
        return true;
    }

    internal bool MoveSim(ISimulable sim, int x, int y) {
        if (!WithinBounds(x, y))
            return false;

        (int ox, int oy) = sim.Position;
        sim.Position = (x, y);

        _cells[ox, oy].Remove(sim);
        _cells[x, y].Add(sim);

        return true;
    }

    internal bool RemoveSim(ISimulable sim) {
        (int x, int y) = sim.Position;
        if (!WithinBounds(x, y))
            return false;

        _cells[x, y].Remove(sim);
        return true;
    }
}
