using DatabaseTesterWebAPI.Services;
using DatabaseTests.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tester.Utils;

namespace DatabaseTesterWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrivateEndpointController : ControllerBase
    {
        private readonly IHttpClientInsertsService _httpClientInsertsService;

        public PrivateEndpointController(IHttpClientInsertsService httpClientInsertsService)
        {
            _httpClientInsertsService = httpClientInsertsService;
        }

        [HttpPost("RangeAddByBatchingAsync")]
        public async Task<ActionResult<User>> RangeAddByBatchingAsync(IEnumerable<User> users)
        {
            await _httpClientInsertsService.InsertByEndpoint(users);
            return Ok();
        }

        [HttpPost("RangeAddAsync")]
        public async Task<ActionResult<User>> RangeAddAsync(IEnumerable<User> users)
        {
            await _httpClientInsertsService.AddByRangeAsync(users);
            return Ok();
        }
    }
}
