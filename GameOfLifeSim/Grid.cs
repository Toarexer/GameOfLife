using System.Collections;

namespace GameOfLifeSim;

/// <summary>Wraps an array of lists to form a grid and store the Sims of the simulation that implement <see cref="ISimulable"/>.</summary>
public class Grid : IEnumerable<IReadOnlyList<ISimulable>> {
    private sealed class GridEnumerator : IEnumerator<IReadOnlyList<ISimulable>> {
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

    /// <summary>The Width of the Grid.</summary>
    public int Width { get; }

    /// <summary>The Height of the Grid.</summary>
    public int Height { get; }

    /// <summary>The number of Sims that can occupy the same cell.</summary>
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

    /// <param name="x">The horizontal position within the <see cref="Grid"/> that starts from 0.</param>
    /// <param name="y">The vertical position within the <see cref="Grid"/> that starts from 0.</param>
    /// <summary>Checks if the the coordinates fall within the <see cref="Grid"/>.</summary>
    /// <returns>True if the coordinates are within the <see cref="Grid"/>, otherwise False.</returns>
    public bool WithinBounds(int x, int y) => x >= 0 && x < Width && y >= 0 && y < Height;

    /// <param name="p">The <see cref="GridPosition"/> that stores the horizontal and vertical positions that start from 0.</param>
    /// <summary>Checks if the <see cref="GridPosition"/> falls within the <see cref="Grid"/>.</summary>
    /// <returns>True if the <see cref="GridPosition"/> is within the <see cref="Grid"/>, otherwise False.</returns>
    public bool WithinBounds(GridPosition p) => WithinBounds(p.X, p.Y);

    /// <param name="x">The horizontal position within the <see cref="Grid"/> that starts from 0.</param>
    /// <param name="y">The vertical position within the <see cref="Grid"/> that starts from 0.</param>
    /// <returns>
    /// An <see cref="IReadOnlyList{T}"/> filled with the <see cref="ISimulable"/> Sims on the specified <paramref name="x"/> and <paramref name="y"/> position.
    /// Returns an empty <see cref="IReadOnlyList{T}"/> if there are no Sims at the position or the position is out of bounds.
    /// </returns>
    public IReadOnlyList<ISimulable> this[int x, int y] {
        get {
            if (WithinBounds(x, y))
                return _cells[PosToIndex(x, y)].AsReadOnly();
            return new List<ISimulable>().AsReadOnly();
        }
    }

    /// <param name="x">The horizontal position within the <see cref="Grid"/> that starts from 0.</param>
    /// <param name="y">The vertical position within the <see cref="Grid"/> that starts from 0.</param>
    /// <typeparam name="T">An object that implements <see cref="ISimulable"/>.</typeparam>
    /// <returns>
    /// An <see cref="IEnumerable{T}"/> filled with the <see cref="ISimulable"/> Sims that are of type <typeparamref name="T"/> on the specified <paramref name="x"/> and <paramref name="y"/> position.
    /// Returns no elements if there are no Sims at the position that are of type <typeparamref name="T"/> or the position is out of bounds.
    /// </returns>
    public IEnumerable<T> SimsOfType<T>(int x, int y) where T : ISimulable {
        return this[x, y].OfType<T>();
    }

    /// <param name="ox">The horizontal position of the origin within the <see cref="Grid"/> that starts from 0.</param>
    /// <param name="oy">The vertical position of the origin within the <see cref="Grid"/> that starts from 0.</param>
    /// <param name="r">The radius of the search. A radius of 0 will only check the origin.</param>
    /// <typeparam name="T">An object that implements <see cref="ISimulable"/>.</typeparam>
    /// <returns>
    /// An <see cref="IEnumerable{T}"/> filled with the <see cref="ISimulable"/> Sims that are of type <typeparamref name="T"/> around the specified <paramref name="ox"/> and <paramref name="oy"/> position.
    /// Returns no elements if there are no Sims around the position that are of type <typeparamref name="T"/> or all positions are out of bounds.
    /// </returns>
    public IEnumerable<T> SimsOfTypeInRadius<T>(int ox, int oy, int r) where T : ISimulable {
        for (int y = oy - r; y < oy + r; y++)
            for (int x = ox - r; x < ox + r; x++)
                foreach (T sim in this[x, y].OfType<T>())
                    yield return sim;
    }

    /// <param name="ox">The horizontal position of the origin within the <see cref="Grid"/> that starts from 0.</param>
    /// <param name="oy">The vertical position of the origin within the <see cref="Grid"/> that starts from 0.</param>
    /// <param name="r">The radius of the search. A radius of 0 will only check the origin.</param>
    /// <returns>
    /// An <see cref="IEnumerable{T}"/> filled with the <see cref="ISimulable"/> Sims around the specified <paramref name="ox"/> and <paramref name="oy"/> position.
    /// Returns no elements if there are no Sims around the position or all positions are out of bounds.
    /// </returns>
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
