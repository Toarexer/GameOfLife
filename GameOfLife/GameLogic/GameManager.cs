using System;
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

   private void UpdateSim(Simulable sim) {
      sim.Update(_grid);
                  
      var nextpos = sim.NextPosition;
      if (nextpos is not null)
         _grid.MoveSim(sim, nextpos.Value.x, nextpos.Value.y);

      if (sim.ShouldCreateDescendant(_grid))
         _grid.CreateSim(sim.NewDescendant());

      if (sim.ShouldDie())
         _grid.RemoveSim(sim);
   }
   
   public void Update() {
      for (int y = 0; y < _grid.Height; y++)
         for (int x = 0; x < _grid.Width; x++)
            foreach (Simulable sim in _grid[x, y]) {
               try {
                  UpdateSim(sim);
               }
               catch (Exception e) {
                  // Temporary solution
                  Console.Error.WriteLine(e);
               }
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
