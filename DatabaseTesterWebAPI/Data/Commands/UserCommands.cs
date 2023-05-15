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

        public async Task AddRangeAsync(IEnumerable<User> users)
        {
            await using var dbContextTransaction = await _testerContext.Database.BeginTransactionAsync();
            await _testerContext.Users.AddRangeAsync(users);
            await _testerContext.SaveChangesAsync();
            await dbContextTransaction.CommitAsync();
        }
    }
}
