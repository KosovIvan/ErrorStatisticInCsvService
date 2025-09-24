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
            var errorLogsList = _csvErrorLogsFile.ReadAll();

            var tasks = new List<Task>
            {
                Task.Run(() => _csvErrorLogsFile.AggregateBySeverity(errorLogsList)),
                Task.Run(() => _csvErrorLogsFile.AggregateByProductVersion(errorLogsList)),
                Task.Run(() => _csvErrorLogsFile.AggregateTop10ErrorCodesByProductSeverity(errorLogsList)),
                Task.Run(() => _csvErrorLogsFile.AggregateByHourIntervalsMaxErrorCode(errorLogsList))
            };

            await Task.WhenAll(tasks);

            return Ok();
        }
    }
}