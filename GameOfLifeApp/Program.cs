using Sim = GameOfLifeSim;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GameOfLifeApp ;

internal static class App {
    internal static Sim.GameManager LoadGrid(string file) {
        IEnumerable<string> lines = File.ReadLines(file);

        string[] size = lines.First().Split('x');
        Sim.GameManager gm = new(int.Parse(size[0]), int.Parse(size[1]));

        foreach (string line in lines.Skip(1)) {
            try {
                string[] data = line.Split(',');

                Type type = Type.GetType($"GameOfLife.Entities.{data[0]}, GameOfLife");
                Sim.GridPosition pos = new(int.Parse(data[1]), int.Parse(data[2]));

                object obj = Activator.CreateInstance(type!, pos);
                if (obj is Sim.ISimulable sim)
                    gm.AddSims(sim);
            }
            catch (Exception e) {
                Console.Error.WriteLine(e);
            }
        }

        return gm;
    }

    [STAThread]
    static void Main() {
        Sim.GameManager gm = new(32, 32);

        Gtk.Application.Init();
        new GameManagerWindow("Rabbits and Foxes", gm).ShowAll();
        Gtk.Application.Run();
    }
}
