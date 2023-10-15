using Entities = GameOfLife.Entities;
using Sim = GameOfLifeSim;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameOfLifeApp {
    static class App {
        class GameManagerWindow : Gtk.Window {
            public Sim.GameManager GameManager { get; }

            private readonly Gtk.Paned _paned = new(Gtk.Orientation.Horizontal);
            private readonly Gtk.ListBox _list = new();
            private readonly Gtk.DrawingArea _area = new();

            public GameManagerWindow(string title, Sim.GameManager gm) : base(title) {
                DefaultSize = new(840, 640);
                Title = title;
                GameManager = gm;
                Destroyed += (_, _) => Environment.Exit(0);
                Drawn += (_, _) => {
                    IEnumerable<Sim.DisplayInfo> info = gm.Grid.SelectMany(x => x.Select(y => y.Info())).Distinct();
                    foreach (var child in _list.Children)
                        _list.Remove(child);
                    foreach (var i in info) {
                        Gtk.Label label = new(i.Name);
                        label.ModifyFg(Gtk.StateType.Normal, ColorFromRGB(i.Color.R, i.Color.G, i.Color.B));
                        _list.Add(label);
                    }
                    _list.ShowAll();
                };

                _area.Drawn += DrawGrid;

                _paned.Add1(_list);
                _paned.Add2(_area);
                _paned.Child1.WidthRequest = 200;

                Add(_paned);
            }

            private void DrawGrid(object sender, Gtk.DrawnArgs args) {
                Cairo.Context context = args.Cr;
                context.Scale(_area.AllocatedWidth, _area.AllocatedHeight);

                float cellWidth = 1f / GameManager.Grid.Width;
                float cellHeight = 1f / GameManager.Grid.Height;

                for (int y = 0; y < GameManager.Grid.Height; y++)
                    for (int x = 0; x < GameManager.Grid.Width; x++) {
                        if (GameManager.Grid[x, y].Count > 0) {
                            float h = cellHeight / GameManager.Grid[x, y].Count;
                            for (int i = 0; i < GameManager.Grid[x, y].Count; i++) {
                                var color = GameManager.Grid[x, y][i].Info().Color;
                                context.SetSourceRGB(color.R / 255f, color.G / 255f, color.B / 255f);
                                context.Rectangle(x * cellWidth, (y * cellHeight) + (i * h), cellWidth, h);
                                context.Fill();
                            }
                        }
                        else {
                            if ((x & 1) == (y & 1))
                                context.SetSourceRGB(0.7, 0.7, 0.7);
                            else
                                context.SetSourceRGB(0.9, 0.9, 0.9);
                            context.Rectangle(x * cellWidth, y * cellHeight, cellWidth, cellHeight);
                            context.Fill();
                        }
                    }
            }

            private Gdk.Color ColorFromRGB(int r, int g, int b) {
                return new Gdk.Color { Red = (ushort)(r << 8), Green = (ushort)(g << 8), Blue = (ushort)(b << 8) };
            }
        }

        [STAThread]
        static void Main() {
            Sim.GameManager gm = new(32, 32);
            gm.AddSims(new Entities.Fox(new Sim.GridPosition(7, 0)));
            gm.AddSims(new Entities.Fox(new Sim.GridPosition(3, 3)));
            gm.AddSims(new Entities.Fox(new Sim.GridPosition(31, 8)));
            gm.AddSims(new Entities.Fox(new Sim.GridPosition(31, 31)));
            
            gm.AddSims(new Entities.Rabbit(new Sim.GridPosition(3, 3)));
            gm.AddSims(new Entities.Rabbit(new Sim.GridPosition(17, 17)));
            gm.AddSims(new Entities.Rabbit(new Sim.GridPosition(18, 16)));
            
            gm.AddSims(new Entities.Grass(new Sim.GridPosition(3, 3)));
            gm.AddSims(new Entities.Grass(new Sim.GridPosition(3, 4)));
            gm.AddSims(new Entities.Grass(new Sim.GridPosition(4, 3)));
            gm.AddSims(new Entities.Grass(new Sim.GridPosition(16, 17)));
            gm.AddSims(new Entities.Grass(new Sim.GridPosition(17, 16)));
            gm.AddSims(new Entities.Grass(new Sim.GridPosition(17, 17)));
            gm.AddSims(new Entities.Grass(new Sim.GridPosition(17, 18)));
            gm.AddSims(new Entities.Grass(new Sim.GridPosition(18, 17)));
            gm.AddSims(new Entities.Grass(new Sim.GridPosition(18, 18)));
            
            gm.AddSims(new Entities.Grass(new Sim.GridPosition(13, 13)));
            gm.AddSims(new Entities.Rabbit(new Sim.GridPosition(13, 13)));
            gm.AddSims(new Entities.Fox(new Sim.GridPosition(13, 13)));

            Gtk.Application.Init();
            new GameManagerWindow("Rabbits and Foxes", gm).ShowAll();
            Gtk.Application.Run();
        }
    }
}
