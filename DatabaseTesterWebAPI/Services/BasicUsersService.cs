using DatabaseTests.Models;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Serilog;
using System.Diagnostics;
using Tester.Data;

namespace DatabaseTesterWebAPI.Services
{
    public interface IBasicDbService
    {
        public Task SimpleDatabaseAddAsync(List<User> users);
        public Task SimpleDatabaseAddAutoDetectChangesOffAsync(List<User> users);
        public void SimpleDatabaseAdd(List<User> users);
        public Task AddByRangeAsync(List<User> users);
        public Task AddByRangeAutoDetectChangesOffAsync(List<User> users);

    }


    public class BasicDbService : IBasicDbService
    {
        private readonly TesterContext _testerContext;

        public BasicDbService(TesterContext testerContext)
        {
            _testerContext = testerContext;
        }

        public async Task SimpleDatabaseAddAsync(List<User> users)
        {
            Log.Information($"Database SimpleAdd of {users.Count} users");

            Stopwatch timer = new();
            timer.Start();
            await AddAsync(users);
            timer.Stop();

            Log.Information($"Time: {timer.Elapsed.TotalSeconds}\n");


            async Task AddAsync(List<User> users)
            {
                try
                {
                    foreach (var user in users)
                    {
                        await _testerContext.Users.AddAsync(user);
                    }
                    await _testerContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Log.Information($"Error while saving to database: {ex.Message}\n");
                }
            }
        }

        public async Task SimpleDatabaseAddAutoDetectChangesOffAsync(List<User> users)
        {
            Log.Information($"Database SimpleAdd of {users.Count} users");

            Stopwatch timer = new();
            timer.Start();
            await AddAsync(users);
            timer.Stop();

            Log.Information($"Time: {timer.Elapsed.TotalSeconds}\n");


            async Task AddAsync(List<User> users)
            {
                try
                {
                    foreach (var user in users)
                    {
                        await _testerContext.Users.AddAsync(user);
                    }
                    await _testerContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Log.Information($"Error while saving to database: {ex.Message}\n");
                }
            }
        }

        public void SimpleDatabaseAdd(List<User> users)
        {
            Log.Information($"Database SimpleAdd of {users.Count} users");
            Stopwatch timer = new();
            timer.Start();
            try
            {
                foreach (var user in users)
                {
                    _testerContext.Users.Add(user);
                }
                _testerContext.SaveChanges();
            }
            catch (Exception ex)
            {
                Log.Information($"Error while saving to database: {ex.Message}\n");
            }
            timer.Stop();

            Log.Information($"Time: {timer.Elapsed.TotalSeconds}\n");
        }

        public async Task AddByRangeAsync(List<User> users)
        {
            Log.Information($"Database Add with AddRange() of {users.Count} users");
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

        public async Task AddByRangeAutoDetectChangesOffAsync(List<User> users)
        {
            Log.Information($"Database Add with AddRange() of {users.Count} users");
            Stopwatch timer = new();
            timer.Start();
            try
            {
                _testerContext.ChangeTracker.AutoDetectChangesEnabled = false;
                await _testerContext.Users.AddRangeAsync(users);
                await _testerContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Log.Information($"Error while saving to database: {ex.Message}\n");
            }
            finally
            {
                _testerContext.ChangeTracker.AutoDetectChangesEnabled = true;
            }
            timer.Stop();

            Log.Information($"Time: {timer.Elapsed.TotalSeconds}\n");
        }
    }
}
