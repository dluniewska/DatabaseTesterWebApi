using DatabaseTesterWebAPI.Services;
using DatabaseTests.Models;
using Microsoft.AspNetCore.Mvc;
using Tester.Utils;


namespace DatabaseTesterWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IBasicDbService _basicDbService;
        private readonly IBatchedInsertsService _batchedInsertsService;

        public UsersController(IBasicDbService basicDbService, IBatchedInsertsService batchedInsertsService)
        {
            _basicDbService = basicDbService;
            _batchedInsertsService = batchedInsertsService;
        }

        [HttpPost("SimpleAdd")]
        public async Task<ActionResult<User>> SimpleAdd(int usersCount)
        {
            var users = UsersManager.GetUsers(usersCount);
            await _basicDbService.SimpleDatabaseAddAsync(users);
            return Ok();
        }

        [HttpPost("SimpleAddTrackerOff")]
        public async Task<ActionResult<User>> SimpleAddTrackerOff(int usersCount)
        {
            var users = UsersManager.GetUsers(usersCount);
            await _basicDbService.SimpleDatabaseAddAutoDetectChangesOffAsync(users);
            return Ok();
        }

        [HttpPost("RangeAdd")]
        public async Task<ActionResult<User>> RangeAdd(int usersCount)
        {
            var users = UsersManager.GetUsers(usersCount);
            await _basicDbService.AddByRangeAsync(users);
            return Ok();
        }

        [HttpPost("RangeAddTrackerOff")]
        public async Task<ActionResult<User>> RangeAddTrackerOff(int usersCount)
        {
            var users = UsersManager.GetUsers(usersCount);
            await _basicDbService.AddByRangeAutoDetectChangesOffAsync(users);
            return Ok();
        }

        [HttpPost("BatchedAdd")]
        public async Task<ActionResult<User>> BatchedAdd(int usersCount)
        {
            var users = UsersManager.GetUsers(usersCount);
            await _batchedInsertsService.AddByRangeInBatchesAsync(users);
            return Ok();
        }
    }
}
