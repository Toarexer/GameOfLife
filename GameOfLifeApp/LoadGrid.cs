using Sim = GameOfLifeSim;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace GameOfLifeApp;

public static partial class App {
    public class InvalidLineException : Exception {
        private readonly string _line, _file;
        
        public InvalidLineException(string line, string file) {
            _line = line;
            _file = file;
        }

        public override string Message => $"Invalid line '{_line}' in '{_file}'";
    }
    
    internal static Sim.GameManager LoadGrid(string file) {
        IEnumerable<string> lines = File.ReadLines(file);

        string[]? size = lines.FirstOrDefault()?.Split('x');
        if (size is null || size.Length != 2)
            throw new InvalidLineException(lines.FirstOrDefault(), Path.GetFullPath(file));

        Sim.GameManager gm = new(int.Parse(size[0]), int.Parse(size[1]));

        foreach (string line in lines.Skip(1)) {
            string[] data = line.Split(',');
            if (data.Length != 3)
                throw new InvalidLineException(line, Path.GetFullPath(file));
            
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
