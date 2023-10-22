using Sim = GameOfLifeSim;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace GameOfLifeApp;

public static partial class App {
    public class InvalidLineException : Exception {
        private readonly int _line;
        private readonly string _text;
        private readonly string? _file;

        public InvalidLineException(int line, string text, string? file = null) {
            _line = line;
            _text = text;
            _file = file;
        }

        public override string Message => _file is null
            ? $"Error on line {_line} at '{_text}'"
            : $"Error on line {_line} at '{_text}' in '{_file}'";
    }

    public static Sim.GameManager LoadGrid(IEnumerable<string> lines, string? path = null, bool useFullName = false) {
        string[] filelines = lines.ToArray();

        string[]? size = filelines.FirstOrDefault()?.Split('x');
        if (size is null || size.Length != 2)
            throw new InvalidLineException(1, filelines.FirstOrDefault() ?? string.Empty, path);

        Sim.GameManager gm = new(int.Parse(size[0]), int.Parse(size[1]));

        char[] splitters = { ',', ';' };
        for (int i = 1; i < filelines.Length; i++) {
            string[] data = filelines[i].Split(splitters);
            if (data.Length != 3)
                throw new InvalidLineException(i + 1, filelines[i], path);

            string assemblyname = useFullName
                ? data[0].Split('.')[0]
                : "GameOfLife";
            string typename = useFullName
                ? data[0]
                : "GameOfLife.Entities." + data[0];

            Type? type = Type.GetType($"{typename}, {assemblyname}");
            if (type is null)
                throw new ReflectionTypeLoadException(new[] { type }, null, typename + " does not exist");

            Sim.GridPosition pos = new(int.Parse(data[1]), int.Parse(data[2]));

            object? obj = Activator.CreateInstance(type, pos);
            if (obj is Sim.ISimulable sim)
                gm.AddSims(sim);
            else {
                string isimname = typeof(Sim.ISimulable).FullName ?? nameof(Sim.ISimulable);
                throw new TypeLoadException($"{typename} is not a {isimname}");
            }
        }

        return gm;
    }

    public static Sim.GameManager LoadGrid(string file, bool useFullName = false) {
        return LoadGrid(File.ReadAllLines(file), Path.GetFullPath(file), useFullName);
    }
}
