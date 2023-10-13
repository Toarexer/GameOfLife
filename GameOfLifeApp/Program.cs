using System;
using Gdk;
using Sim = GameOfLifeSim;

namespace GameOfLifeApp {
    static class App {
        static Gdk.Color ColorFromRGB(byte r, byte g, byte b) {
            return new Gdk.Color { Red = (ushort)(r << 8), Green = (ushort)(g << 8), Blue = (ushort)(b << 8) };
        }

        [STAThread]
        static void Main() {
            Sim.GameManager gm = new(32, 32);

            Gtk.Application.Init();

            Gtk.DrawingArea area = new();

            area.Drawn += (_, args) => {
                Cairo.Context context = args.Cr;
                context.Scale(area.AllocatedWidth, area.AllocatedHeight);

                float cellWidth = 1f / gm.Grid.Width;
                float cellHeight = 1f / gm.Grid.Height;

                for (int y = 0; y < gm.Grid.Height; y++)
                    for (int x = 0; x < gm.Grid.Width; x++) {
                        if (gm.Grid[x, y].Count > 0) {
                            float h = cellHeight / gm.Grid[x, y].Count;
                            for (int i = 0; i < gm.Grid[x, y].Count; i++) {
                                var color = gm.Grid[x, y][i].Info().Color;
                                context.SetSourceRGB(color.R, color.G, color.B);
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
            };

            Gtk.Window window = new("Rabbits and Foxes") {
                DefaultSize = new(640, 640),
            };
            window.Destroyed += (_, _) => Environment.Exit(0);
            window.Add(area);
            window.ShowAll();

            Gtk.Application.Run();
        }
    }
}
