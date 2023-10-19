using Sim = GameOfLifeSim;
using System;

namespace GameOfLifeApp;

public static partial class App {
    [STAThread]
    private static void Main() {
        Sim.GameManager gm = new(32, 32);
        Gtk.Application.Init();
        new GameManagerWindow("Rabbits and Foxes", gm).ShowAll();
        Gtk.Application.Run();
    }
}
