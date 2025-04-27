using Serilog;

public static class Batch
{
    public static async Task RunBatchTasksAsync(
        List<Func<Task>> tasks,
        int batchSize = 10,
        int batchTimer = 10
    )
    {
        Log.Information("Running batch tasks");

        for (int i = 0; i < tasks.Count; i += batchSize)
        {
            var batch = tasks.Skip(i).Take(batchSize).ToList();
            var runningTasks = batch.Select(func => func()).ToList();
            await Task.WhenAll(runningTasks);

            if (i + batchSize < tasks.Count)
            {
                Log.Information(
                    "Batch complete, waiting {@batchTime} second before next batch...",
                    batchTimer
                );
                await Task.Delay(TimeSpan.FromSeconds(batchTimer));
            }
        }
    }
}
