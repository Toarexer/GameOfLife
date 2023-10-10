using GameOfLife.Entities.Interfaces;
using GameOfLife.GameLogic;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameOfLife.Entities;

public class Rabbit : Animal, ISimulable, ICanBeEaten {
    public readonly int NutritionalValue = 3;
    private List<ISimulable?> _neighbours = new();

    (int x, int y) _position { get; set; } = (0, 0);
    (int x, int y)? _nextposition { get; set; }

    (int x, int y) ISimulable.Position { get => _position; set => _position = value; }
    (int x, int y)? ISimulable.NextPosition { get => _nextposition; set => _nextposition = value; }

    public Rabbit() {
        Hp = 5;
        Age = 0;
    }

    public Rabbit(int posX, int posY) {
        Hp = 5;
        Age = 0;
        _position = (posX, posY);
    }

    void ISimulable.Update(Grid grid) {
        _neighbours = GetAllNeighbours(grid);
        Move(grid);
        Eat((Grass?)_neighbours.FirstOrDefault(x => x is Grass));
        IncreaseAge(1);
        Hp--;
    }

    bool ISimulable.ShouldCreateDescendant(Grid grid) {
        return false;
    }

    bool ISimulable.ShouldDie() {
        return Hp < 1 || CanBeEaten();
    }

    private List<ISimulable?> GetAllNeighbours(Grid grid) {
        var entities = new List<ISimulable?>();

        for (int i = -2; i < 3; i++)
            for (int j = -2; j < 3; j++) {
                entities.Add(grid.SimsOfType<Grass>(_position.x + i, _position.y + j).FirstOrDefault());
                entities.Add(grid.SimsOfType<Fox>(_position.x + i, _position.y + j).FirstOrDefault());
                entities.Add(grid.SimsOfType<Rabbit>(_position.x + i, _position.y + j).FirstOrDefault());
            }

        return entities;
    }

    private bool CanMove() {
        return _neighbours.Count == 0 || !_neighbours.Any(x => x is Fox);
    }

    private void Move(Grid grid) {
        //Move randomly if hp is full
        if (!ShouldEat() && CanMove()) {
            MoveRandomly(grid);
            return;
        }

        //Move to the next grass to eat else stays, preference by nutritional value
        if (ShouldEat() && CanMove() && _neighbours.Any(x => x is Grass)) {
            _position = _neighbours
                .Where(x => x is Grass)
                .OrderByDescending(y => (Grass?)y)
                .FirstOrDefault(x => x is Grass)?.Position ?? _position;
        }
    }

    private void MoveRandomly(Grid grid) {
        var r = new Random();
        int nextX;
        int nextY;

        do {
            nextX = _position.x + r.Next(-1, 2);
            nextY = _position.y + r.Next(-1, 2);
        }
        while (!grid.WithinBounds(nextX, nextY));

        _position = (nextX, nextY);
    }

    private bool ShouldEat() {
        return Hp < 5;
    }

    public void Eat(Grass? grass) {
        if (ShouldEat() && grass != null) {
            Hp += grass.GetEaten();
        }
    }

    public bool CanBeEaten() {
        //If the predator is on current position, it can be eaten
        return _neighbours.Any(x => x is Fox);
    }

    public void GetEaten() {
        if (!CanBeEaten()) return;
        var fox = (Fox)_neighbours.FirstOrDefault(x => x is Fox)!;
        fox.Eat(this);
    }

    ISimulable ISimulable.NewDescendant(Grid grid) {
        throw new NotImplementedException();
    }
}