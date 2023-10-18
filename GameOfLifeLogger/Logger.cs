namespace GameOfLifeLogger;

public static class Logger {
    public static List<Action<string>> InfoLoggers { get; } = new();
    public static List<Action<string>> ErrorLoggers { get; } = new();

    public static void Info(string format, params object?[] args) {
        foreach (Action<string> logger in InfoLoggers)
            logger($"[{GetTime()}] Info | {string.Format(format, args)}");
    }

    public static void Error(string format, params object?[] args) {
        foreach (Action<string> logger in ErrorLoggers)
            logger($"[{GetTime()}] Err  | {string.Format(format, args)}");
    }

    private static string GetTime() {
        DateTime t = DateTime.Now;
        return $"{t.Hour:D2}:{t.Minute:D2}:{t.Second:D2}.{t.Millisecond:D4}";
    }
}
