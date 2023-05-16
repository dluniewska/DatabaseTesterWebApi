using DatabaseTesterWebAPI.Data.Commands;
using DatabaseTests.Models;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Diagnostics;
using Tester.Data;

namespace DatabaseTesterWebAPI.Services
{
    public interface IBatchedInsertsService
    {
        public Task AddByRangeInBatchesWithSyncInsert(List<User> users);
        public Task AddByRangeInBatchesWithAsyncInsert(List<User> users);
    }

    public class BatchedInsertsService : IBatchedInsertsService
    {
        private readonly IServiceProvider _services;
        private readonly IDbContextFactory<TesterContext> _contextFactory;

        public BatchedInsertsService(IServiceProvider services, IDbContextFactory<TesterContext> contextFactory)
        {
            _services = services;
            _contextFactory = contextFactory;
        }

        public async Task AddByRangeInBatchesWithSyncInsert(List<User> users)
        {
            Log.Information($"Database add with sync add action and with batches of {users.Count} users");
            Stopwatch timer = new();
            timer.Start();
            try
            {
                var tasks = new List<Task>();
                var batchSize = 1000;
                int numberOfBatches = (int)Math.Ceiling((double)users.Count / batchSize);

                for (int i = 0; i < numberOfBatches; i++)
                {
                    using var batchContext = _contextFactory.CreateDbContext();
                    var currentBatch = users.Skip(i * batchSize).Take(batchSize);
                    tasks.Add(new UserCommands(batchContext).AddRange(currentBatch));
                }
                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                Log.Information($"Error while saving to database: {ex.Message}\n");
            }
            timer.Stop();

            Log.Information($"Time: {timer.Elapsed.TotalSeconds}\n");
        }

        public async Task AddByRangeInBatchesWithAsyncInsert(List<User> users)
        {
            Log.Information($"Database add with async add action and with batches of {users.Count} users");
            Stopwatch timer = new();
            timer.Start();
            try
            {
                var tasks = new List<Task>();
                var batchSize = 100;
                int numberOfBatches = (int)Math.Ceiling((double)users.Count / batchSize);

                for (int i = 0; i < numberOfBatches; i++)
                {
                    using var batchContext = _contextFactory.CreateDbContext();
                    var currentBatch = users.Skip(i * batchSize).Take(batchSize);
                    tasks.Add(new UserCommands(batchContext).AddRangeAsync(currentBatch));
                }
                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                Log.Information($"Error while saving to database: {ex.Message}\n");
            }
            timer.Stop();

            Log.Information($"Time: {timer.Elapsed.TotalSeconds}\n");
        }
    }
}
