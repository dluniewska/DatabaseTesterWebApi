using DatabaseTesterWebAPI.Services;
using DatabaseTests.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using Tester.Utils;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DatabaseTesterWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IBasicDbService _basicDbService;

        public UsersController(IBasicDbService basicDbService)
        {
            _basicDbService = basicDbService;
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
    }
}
