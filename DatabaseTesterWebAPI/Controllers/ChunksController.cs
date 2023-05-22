using DatabaseTesterWebAPI.Services;
using DatabaseTests.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tester.Utils;

namespace DatabaseTesterWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChunksController : ControllerBase
    {
        private readonly IChunkedInsertsService _chunkedInsertsService;

        public ChunksController(IChunkedInsertsService chunkedInsertsService)
        {
            _chunkedInsertsService = chunkedInsertsService;
        }

        [HttpPost("ChunkedAsyncInsert")]
        public async Task<ActionResult<User>> ChunkedAsyncInsert(int usersCount)
        {
            var users = UsersManager.GetUsers(usersCount);
            await _chunkedInsertsService.AddInChunksWithAsyncInsert(users);
            return Ok();
        }
    }
}
