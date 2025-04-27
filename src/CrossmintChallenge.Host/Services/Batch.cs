using Serilog;

public static class Batch
{
    public static async Task RunBatchTasksAsync(List<Task> tasks, int batchSize = 10)
    {
        Log.Information("Running batch tasks");

        for (int i = 0; i < tasks.Count; i += batchSize)
        {
            var batch = tasks.Skip(i).Take(batchSize).ToList();

            await Task.WhenAll(batch);

            if (i + batchSize < tasks.Count)
            {
                Log.Information("Batch complete, waiting 1 second before next batch...");
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }
    }
}
