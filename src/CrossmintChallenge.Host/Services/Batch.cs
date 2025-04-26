using Serilog;
public static class Batch
{

    public static async Task CallStarsAsync(
        List<Task> tasks,
        int batchSize = 10
    )
    {
        Log.Information("calling tasks {@tasks}", tasks);

        for (int i = 0; i < tasks.Count; i += batchSize)
        {
            var batch = tasks.Skip(i).Take(batchSize).ToList();
            // var tasks = batch
            //     .Select(starTask => starTask(ChallengeUrl(), starTask.Row, starTask.Col, CandidateId))
            //     .ToList();

            await Task.WhenAll(batch);

            if (i + batchSize < tasks.Count)
            {
                Log.Information("Batch complete, waiting 1 seconds before next batch...");
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }
    }
}
