using DatabaseTesterWebAPI.Services;
using DatabaseTests.Models;
using Microsoft.AspNetCore.Mvc;
using Tester.Utils;


namespace DatabaseTesterWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EFSimpleController : ControllerBase
    {
        private readonly IBasicDbService _basicDbService;
        private readonly IBatchedInsertsService _batchedInsertsService;

        public EFSimpleController(IBasicDbService basicDbService, IBatchedInsertsService batchedInsertsService)
        {
            _basicDbService = basicDbService;
            _batchedInsertsService = batchedInsertsService;
        }

        [HttpPost("SimpleAdd")]
        public async Task<ActionResult<User>> SimpleAdd(int usersCount)
        {
            var users = UsersManager.GetUsers(usersCount);
            _basicDbService.SimpleDatabaseAdd(users);
            return Ok();
        }

        [HttpPost("SimpleAddTrackerOff")]
        public async Task<ActionResult<User>> SimpleAddTrackerOff(int usersCount)
        {
            var users = UsersManager.GetUsers(usersCount);
            _basicDbService.SimpleDatabaseAddAutoDetectChangesOff(users);
            return Ok();
        }

        [HttpPost("SimpleAddAsync")]
        public async Task<ActionResult<User>> SimpleAddAsync(int usersCount)
        {
            var users = UsersManager.GetUsers(usersCount);
            await _basicDbService.SimpleDatabaseAddAsync(users);
            return Ok();
        }

        [HttpPost("SimpleAddTrackerOffAsync")]
        public async Task<ActionResult<User>> SimpleAddTrackerOffAsync(int usersCount)
        {
            var users = UsersManager.GetUsers(usersCount);
            await _basicDbService.SimpleDatabaseAddAutoDetectChangesOffAsync(users);
            return Ok();
        }

        [HttpPost("RangeAddAsync")]
        public async Task<ActionResult<User>> RangeAddAsync(int usersCount)
        {
            var users = UsersManager.GetUsers(usersCount);
            await _basicDbService.AddByRangeAsync(users);
            return Ok();
        }

        [HttpPost("RangeAddTrackerOffAsync")]
        public async Task<ActionResult<User>> RangeAddTrackerOffAsync(int usersCount)
        {
            var users = UsersManager.GetUsers(usersCount);
            await _basicDbService.AddByRangeAutoDetectChangesOffAsync(users);
            return Ok();
        }

        [HttpPost("BatchedAddAsyncInsert")]
        public async Task<ActionResult<User>> BatchedAddAsyncInsert(int usersCount)
        {
            var users = UsersManager.GetUsers(usersCount);
            await _batchedInsertsService.AddByRangeInBatchesWithAsyncInsert(users);
            return Ok();
        }

        [HttpPost("BatchedAddSyncInsert")]
        public async Task<ActionResult<User>> BatchedAddSyncInsert(int usersCount)
        {
            var users = UsersManager.GetUsers(usersCount);
            await _batchedInsertsService.AddByRangeInBatchesWithSyncInsert(users);
            return Ok();
        }
    }
}
