using System.Text;
using System.Transactions;

namespace GameOfLifeSimTest;

using GameOfLifeSim;

class DummyPair {
    public Dummy First { get; init; }
    public Dummy Second { get; init; }
}

class Dummy : ISimulable {
    public GridPosition Position { get; set; } = GridPosition.Zero;
    public GridPosition? NextPosition { get; set; }

    public int Health { get; set; } = 100;
    public bool CanMate { get; set; } = false;
    public DummyPair? Mating { get; set; } = null;

    void ISimulable.Update(Grid grid) {
        if (Mating is not null && (!Mating.First.CanMate || !Mating.Second.CanMate)) {
            CanMate = false;
            Mating = null;
        }

        if (CanMate && Mating is null) {
            IEnumerable<Dummy> sims = grid.SimsOfTypeInRadius<Dummy>(Position.X, Position.Y, 2)
                .Except(this)
                .Where(x => x.CanMate && x.Mating is null);

            if (sims.FirstOrDefault() is Dummy other) {
                Mating = new DummyPair { First = this, Second = other };
                other.Mating = Mating;
            }
        }
    }

    bool ISimulable.ShouldDie() {
        return Health <= 0;
    }

    ISimulable? ISimulable.NewDescendant(Grid grid) {
        if (CanMate && Mating is not null && Mating.First == this && Position == Mating.Second.Position)
            return null;
        CanMate = false;
        return new Dummy { Position = Position };
    }
}
