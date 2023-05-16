using DatabaseTesterWebAPI.Services;
using DatabaseTests.Models;
using Microsoft.AspNetCore.Mvc;
using Tester.Utils;

namespace DatabaseTesterWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NpgSqlController : ControllerBase
    {
        private readonly INpgSqlBulkInsertService _npgSqlBulkInsertService;

        public NpgSqlController(INpgSqlBulkInsertService npgSqlBulkInsertService)
        {
            _npgSqlBulkInsertService = npgSqlBulkInsertService;
        }

        [HttpPost("WriteToServerWithNpgSql")]
        public async Task<ActionResult<User>> WriteToServerWithNpgSql(int usersCount, string destinationTable)
        {
            var users = UsersManager.GetUsers(usersCount);
            _npgSqlBulkInsertService.WriteToServer(users, destinationTable);
            return Ok();
        }

        [HttpPost("BulkInsertBinaryImport")]
        public async Task<ActionResult<User>> BulkInsertBinaryImport(int usersCount)
        {
            var users = UsersManager.GetUsers(usersCount);
            await _npgSqlBulkInsertService.BulkInsertBinaryImporter(users);
            return Ok();
        }
    }
}
