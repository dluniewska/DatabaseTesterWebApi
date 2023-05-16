using DatabaseTesterWebAPI.Data.Commands;
using DatabaseTests.Models;
using Microsoft.EntityFrameworkCore.Internal;
using Serilog;
using System.Diagnostics;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;
using Tester.Data;

namespace DatabaseTesterWebAPI.Services
{
    public interface IHttpClientInsertsService
    {
        public Task InsertByEndpoint(IEnumerable<User> users);
        public Task AddByRangeAsync(IEnumerable<User> users);
    }

    public class HttpClientInsertsService : IHttpClientInsertsService
    {
        private readonly TesterContext _testerContext;
        private readonly IHttpClientFactory _httpClientFactory;
        private string contentType = "application/json";

        public HttpClientInsertsService(TesterContext testerContext, IHttpClientFactory httpClientFactory)
        {
            _testerContext = testerContext;
            _httpClientFactory = httpClientFactory;
        }

        public async Task AddByRangeAsync(IEnumerable<User> users)
        {
            Log.Information($"Database private Add with AddRangeAsync() of {users.Count()} users");
            Stopwatch timer = new();
            timer.Start();
            try
            {
                await _testerContext.Users.AddRangeAsync(users);
                await _testerContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Log.Information($"Error while saving to database: {ex.Message}\n");
            }
            timer.Stop();

            Log.Information($"Time: {timer.Elapsed.TotalSeconds}\n");
        }

        public async Task InsertByEndpoint(IEnumerable<User> users)
        {
            Log.Information($"Insert by endpoint of {users.Count()} users");
            Stopwatch timer = new();
            timer.Start();
            try
            {
                var batchSize = 1000;
                int numberOfBatches = (int)Math.Ceiling((double)users.Count() / batchSize);
                using var httpClient = _httpClientFactory.CreateClient();

                for (int i = 0; i < numberOfBatches; i++)
                {
                    var currentBatch = users.Skip(i * batchSize).Take(batchSize);
                    var httpContent = new StringContent(JsonSerializer.Serialize(currentBatch), Encoding.UTF8, contentType);
                    var response = await httpClient.PostAsync($"https://localhost:7262/api/PrivateEndpoint/RangeAddAsync", httpContent);
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
