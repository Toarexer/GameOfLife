using GameOfLifeSim;

namespace GameOfLife.Entities;

public class MatingPair<T> where T: ISimulable
{
    public T MatingPair1 { get; init; }
    public T MatingPair2 { get; init; }

    public MatingPair(T matingPair1, T matingPair2)
    {
        MatingPair1 = matingPair1;
        MatingPair2 = matingPair2;
    }
    
    
}