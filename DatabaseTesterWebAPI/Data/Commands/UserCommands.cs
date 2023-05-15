using DatabaseTests.Models;
using Tester.Data;

namespace DatabaseTesterWebAPI.Data.Commands
{
    public class UserCommands
    {
        private readonly TesterContext _testerContext;

        public UserCommands(TesterContext testerContext)
        {
            _testerContext = testerContext;
        }

        public Task AddRange(IEnumerable<User> users)
        {
            _testerContext.Users.AddRange(users);
            _testerContext.SaveChanges();
            return Task.CompletedTask;
        }

        public async Task AddRangeAsync(IEnumerable<User> users)
        {
            await _testerContext.Users.AddRangeAsync(users);
            await _testerContext.SaveChangesAsync();
        }
    }
}
