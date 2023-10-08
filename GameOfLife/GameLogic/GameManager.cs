using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GameOfLife.GameLogic;

public class GameManager {
   private readonly Grid _grid;

   public GameManager(int gridWidth, int gridHeight, params Simulable[] sims) {
      _grid = new(gridWidth, gridHeight);
      foreach (Simulable sim in sims)
         _grid.CreateSim(sim);
   }
   
   private void UpdateSims() {
      for (int y = 0; y < _grid.Height; y++)
         for (int x = 0; x < _grid.Width; x++)
            foreach (Simulable sim in _grid[x, y])
               sim.Update(_grid);
   }

   private void KillSims() {
      for (int y = 0; y < _grid.Height; y++)
         for (int x = 0; x < _grid.Width; x++)
            foreach (Simulable sim in _grid[x, y].Where(sim => sim.ShouldDie()))
               _grid.RemoveSim(sim);
   }
   
   private void ReproduceSims() {
      for (int y = 0; y < _grid.Height; y++)
         for (int x = 0; x < _grid.Width; x++)
            foreach (Simulable sim in _grid[x, y].Where(sim => sim.ShouldCreateDescendant(_grid)))
               _grid.CreateSim(sim.NewDescendant());
   }
   
   private void MoveSims() {
      for (int y = 0; y < _grid.Height; y++)
         for (int x = 0; x < _grid.Width; x++)
            foreach (Simulable sim in _grid[x, y].Where(sim => sim.NextPosition is not null))
               _grid.MoveSim(sim, sim.NextPosition!.Value.x, sim.NextPosition!.Value.y);
   }

   public void Update() {
      try {
         UpdateSims();
         KillSims();
         ReproduceSims();
         MoveSims();
      }
      catch (Exception e) {
         // Temporary solution
         Console.Error.WriteLine(e);
      }
   }

   public async Task Run(CancellationToken ctoken, int msInterval = 1000, int times = 0) {
      try {
         if (times == 0)
            while (!ctoken.IsCancellationRequested) {
               Update();
               await Task.Delay(msInterval, ctoken);
            }
         else
            for (int i = 0; i < times; i++) {
               if (ctoken.IsCancellationRequested)
                  break;
               Update();
               await Task.Delay(msInterval, ctoken);
            }
      }
      catch (TaskCanceledException) {
      }
   }
}
