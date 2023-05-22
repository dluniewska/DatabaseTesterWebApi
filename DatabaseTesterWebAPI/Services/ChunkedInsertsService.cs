using DatabaseTesterWebAPI.Controllers;
using DatabaseTesterWebAPI.Data.Commands;
using DatabaseTests.Models;
using Microsoft.EntityFrameworkCore.Internal;
using Serilog;
using System.Diagnostics;
using System.Reflection.Emit;
using Tester.Data;
using static DatabaseTesterWebAPI.Utils.ChunksExtension;

namespace DatabaseTesterWebAPI.Services
{
    public interface IChunkedInsertsService
    {
        public Task AddInChunksWithAsyncInsert(List<User> users);
    }

    public class ChunkedInsertsService : IChunkedInsertsService
    {
        private readonly TesterContext _testerContext;

        public ChunkedInsertsService(TesterContext testerContext)
        {
            _testerContext = testerContext;
        }

        public async Task AddInChunksWithAsyncInsert(List<User> users)
        {
            var objects = users.Chunk(200);
            Log.Information($"Database add with async add action and with batches of {users.Count} users");
            Stopwatch timer = new();
            timer.Start();
            try
            {
                var chunks = new Chunks(users.Count, 1000);
                Log.Information("{ChunkCount} of Chunks To Initialize", chunks.TotalChunks);
                int skip = 0;
                foreach (var (index, value) in chunks)
                {
                    skip += value;
                    Log.Information($"#{index}: Inserting {value} rows of objects", index, value);
                    var records = users.Skip(skip).Take(value);
                    await _testerContext.Users.AddRangeAsync(records);
                    await _testerContext.SaveChangesAsync();
                    _testerContext.ChangeTracker.Clear();
                }
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
