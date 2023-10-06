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
