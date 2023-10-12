namespace GameOfLifeSim;

public static class Utils {
    public static IEnumerable<T> Except<T>(this IEnumerable<T> collection, ISimulable sim) where T : ISimulable {
        return collection.Where(x => x as ISimulable != sim);
    }
}
