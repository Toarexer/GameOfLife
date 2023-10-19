using Sim = GameOfLifeSim;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace GameOfLifeApp;

public static partial class App {
    public class InvalidLineException : Exception {
        private readonly int _line;
        private readonly string _text, _file;

        public InvalidLineException(int line, string text, string file) {
            _line = line;
            _text = text;
            _file = file;
        }

        public override string Message => $"Error on line {_line} at '{_text}' in '{_file}'";
    }

    internal static Sim.GameManager LoadGrid(string file) {
        string[] lines = File.ReadLines(file).ToArray();

        string[]? size = lines.FirstOrDefault()?.Split('x');
        if (size is null || size.Length != 2)
            throw new InvalidLineException(1, lines.FirstOrDefault() ?? string.Empty, Path.GetFullPath(file));

        Sim.GameManager gm = new(int.Parse(size[0]), int.Parse(size[1]));

        char[] splitters =  { ',', ';' };
        for (int i = 1; i < lines.Length; i++) {
            string[] data = lines[i].Split(splitters);
            if (data.Length != 3)
                throw new InvalidLineException(i + 1, lines[i], Path.GetFullPath(file));

            Type? type = Type.GetType($"GameOfLife.Entities.{data[0]}, GameOfLife");
            if (type is null)
                throw new ReflectionTypeLoadException(new[] { type }, null, $"GameOfLife.Entities.{data[0]}");

            Sim.GridPosition pos = new(int.Parse(data[1]), int.Parse(data[2]));

            object? obj = Activator.CreateInstance(type, pos);
            if (obj is Sim.ISimulable sim)
                gm.AddSims(sim);
            else {
                string isimname = typeof(Sim.ISimulable).FullName ?? nameof(Sim.ISimulable);
                throw new TypeLoadException($"GameOfLife.Entities.{data[0]} is not {isimname}");
            }
        }

        return gm;
    }
}
