namespace GameOfLife.GameLogic;

public class GameManager {
   private readonly Grid _grid;

   public GameManager(int gridWidth, int gridHeight, params ISimulable[] sims) {
      _grid = new(gridWidth, gridHeight);
      foreach (ISimulable sim in sims)
         _grid.CreateSim(sim);
   }

   public void Run() {
      for (int y = 0; y < _grid.Height; y++)
         for (int x = 0; x < _grid.Width; x++)
            foreach (ISimulable sim in _grid[x, y]) {
               (int nx, int ny) = sim.Move(_grid);
               _grid.MoveSim(sim, nx, ny);

               sim.Update(_grid);

               if (sim.ShouldCreateDescendant(_grid))
                  _grid.CreateSim(sim.NewDescendant());
               
               if(sim.ShouldDie())
                  _grid.RemoveSim(sim);
            }
   }
}
