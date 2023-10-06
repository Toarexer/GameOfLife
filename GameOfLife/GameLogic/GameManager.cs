namespace GameOfLife.GameLogic;

public interface ISimulable {
   public (int x, int y) Position();
   public void SetPosition(int x, int y);
   
   public (int x, int y) Move(Grid grid);
   public void Update(Grid grid);
   
   public bool ShouldCreateDescendant(Grid grid);
   public bool ShouldDie();
   
   public ISimulable NewDescendant();
}

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
