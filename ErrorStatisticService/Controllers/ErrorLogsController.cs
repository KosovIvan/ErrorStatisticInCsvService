using ErrorStatisticService.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ErrorStatisticService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ErrorLogsController : ControllerBase
    {
        private readonly CsvErrorLogsFile _csvErrorLogsFile;
        public ErrorLogsController(CsvErrorLogsFile csvErrorLogsFile)
        {
            _csvErrorLogsFile = csvErrorLogsFile;
        }

        [HttpPost("generate")]
        public IActionResult GenerateData()
        {
            _csvErrorLogsFile.Init();
            return Ok();
        }

        [HttpPost("aggregate")]
        public async Task<IActionResult> AggregateData()
        {

            return Ok();
        }
    }
}