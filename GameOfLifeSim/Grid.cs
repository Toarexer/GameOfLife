using System.Collections;

namespace GameOfLifeSim;

public class Grid : IEnumerable<IReadOnlyList<ISimulable>> {
    private class GridEnumerator : IEnumerator<IReadOnlyList<ISimulable>> {
        private readonly Grid _grid;
        private int _index = -1;

        public GridEnumerator(Grid grid) => _grid = grid;

        public bool MoveNext() => ++_index < _grid.Width * _grid.Height;

        public void Reset() => _index = -1;

        public IReadOnlyList<ISimulable> Current => _grid._cells[_index];

        object IEnumerator.Current => Current;

        public void Dispose() {
        }
    }

    private readonly List<ISimulable>[] _cells;

    public int Width { get; }
    public int Height { get; }
    
    public int CellCapacity { get; }

    internal Grid(int width, int height, int cellcap) {
        _cells = new List<ISimulable>[width * height];
        for (int i = 0; i < _cells.Length; i++)
            _cells[i] = new();
        
        Width = width;
        Height = height;
        CellCapacity = cellcap;
    }

    public IEnumerator<IReadOnlyList<ISimulable>> GetEnumerator() => new GridEnumerator(this);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private int PosToIndex(int x, int y) => y * Width + x;
    
    public bool WithinBounds(int x, int y) => x >= 0 && x < Width && y >= 0 && y < Height;

    public bool WithinBounds(GridPosition p) => WithinBounds(p.X, p.Y);

    public IReadOnlyList<ISimulable> this[int x, int y] {
        get {
            if (WithinBounds(x, y))
                return _cells[PosToIndex(x, y)].AsReadOnly();
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
    
    internal bool CreateSim(ISimulable sim, GridPosition pos) {
        if (!WithinBounds(pos) || _cells[PosToIndex(pos.X, pos.Y)].Count >= CellCapacity)
            return false;
        
        _cells[PosToIndex(pos.X, pos.Y)].Add(sim);
        sim.Position = pos;
        
        return true;
    }

    internal bool CreateSim(ISimulable sim) {
        return CreateSim(sim, sim.Position);
    }

    internal bool MoveSim(ISimulable sim, GridPosition from, GridPosition to) {
        if (!RemoveSim(sim, from))
            return false;

        return CreateSim(sim, to);
    }

    internal bool RemoveSim(ISimulable sim, GridPosition pos) {
        if (!WithinBounds(pos))
            return false;
        
        return _cells[PosToIndex(pos.X, pos.Y)].Remove(sim);
    }
}
