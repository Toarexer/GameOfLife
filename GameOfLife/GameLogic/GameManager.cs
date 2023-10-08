using System.Threading;
using System.Threading.Tasks;

namespace GameOfLife.GameLogic;

public class GameManager {
   private readonly Grid _grid;

   public GameManager(int gridWidth, int gridHeight, params ISimulable[] sims) {
      _grid = new(gridWidth, gridHeight);
      foreach (ISimulable sim in sims)
         _grid.CreateSim(sim);
   }

   public void Update() {
      for (int y = 0; y < _grid.Height; y++)
         for (int x = 0; x < _grid.Width; x++)
            foreach (ISimulable sim in _grid[x, y]) {
               (int nx, int ny) = sim.Move(_grid);
               _grid.MoveSim(sim, nx, ny);

               sim.Update(_grid);

               if (sim.ShouldCreateDescendant(_grid))
                  _grid.CreateSim(sim.NewDescendant());

               if (sim.ShouldDie())
                  _grid.RemoveSim(sim);
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
