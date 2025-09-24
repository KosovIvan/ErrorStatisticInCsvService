using CsvHelper.Configuration;
using CsvHelper;
using ErrorStatisticService.Models;
using ErrorStatisticService.Services;
using System.Globalization;

namespace ErrorStatisticService.Data
{
    public class CsvErrorLogsFile
    {
        private const int upperRowsAmountBound = 1000000;
        private readonly int rowsAmount;
        private readonly Random rnd = new Random();
        private readonly IRandomGeneratorService<ErrorLog> _randomErrorLogGeneratorService;
        
        public CsvErrorLogsFile(IRandomGeneratorService<ErrorLog> randomErrorLogGeneratorService)
        {
            rowsAmount = rnd.Next(10000, upperRowsAmountBound);
            _randomErrorLogGeneratorService = randomErrorLogGeneratorService;
        }

        public void Init()
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true
            };
            using var writer = new StreamWriter("errors.csv", false);
            using var csv = new CsvWriter(writer, config);

            csv.WriteHeader<ErrorLog>();
            csv.NextRecord();

            for (int i = 0; i < rowsAmount; i++)
            {
                var errorLog = _randomErrorLogGeneratorService.Generate();
                csv.WriteRecord(errorLog);
                csv.NextRecord();
            }
        }

        public List<ErrorLog> ReadAll()
        {
            using var reader = new StreamReader("errors.csv");
            using var csv = new CsvHelper.CsvReader(reader, CultureInfo.InvariantCulture);

            var errorLogs = csv.GetRecords<ErrorLog>().ToList();

            return errorLogs;
        }

        public async Task WriteAll<T>(IEnumerable<T> records, string fileName)
        {
            await using var writer = new StreamWriter(fileName, false);
            await using var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true
            });

            await csv.WriteRecordsAsync(records);
        }


        public async Task AggregateBySeverity(List<ErrorLog> errorLogs)
        {
            var aggregatedList = errorLogs
                .GroupBy(e => e.Severity)
                .Select(g => new 
                { 
                    Severity = g.Key, 
                    Count = g.Count() 
                })
                .OrderByDescending(g => g.Count);

            await WriteAll(aggregatedList, "errors_by_severity.csv");
        }

        public async Task AggregateByProductVersion(List<ErrorLog> errorLogs)
        {
            var aggregatedList = errorLogs
                .GroupBy(e => new 
                { 
                    e.Product,
                    e.Version 
                })
                .Select(g => new 
                { 
                    Product = g.Key.Product,
                    Version = g.Key.Version,
                    Count = g.Count() 
                })
                .OrderByDescending(g => g.Count);

            await WriteAll(aggregatedList, "errors_by_product_version.csv");
        }

        public async Task AggregateTop10ErrorCodesByProductSeverity(List<ErrorLog> errorLogs)
        {
            var aggregatedList = errorLogs
                .GroupBy(e => new { e.Product, e.Severity, e.ErrorCode })
                .Select(g => new
                {
                    g.Key.Product,
                    g.Key.Severity,
                    g.Key.ErrorCode,
                    Count = g.Count()
                })
                .GroupBy(x => new { x.Product, x.Severity })
                .SelectMany(g => g.OrderByDescending(x => x.Count).Take(10));

            await WriteAll(aggregatedList, "errors_by_product_severity.csv");
        }

        public async Task AggregateByHourIntervalsMaxErrorCode(List<ErrorLog> errorLogs)
        {
            var aggregatedList = errorLogs
                .GroupBy(e => new
                {
                    Hour = e.Timestamp.Hour,
                    e.Product,
                    e.Version,
                    e.ErrorCode
                })
                .Select(g => new
                {
                    g.Key.Hour,
                    g.Key.Product,
                    g.Key.Version,
                    g.Key.ErrorCode,
                    Count = g.Count()
                })
                .GroupBy(x => new { x.Hour, x.Product, x.Version })
                .Select(g => g.OrderByDescending(x => x.Count).First());

            await WriteAll(aggregatedList, "errors_by_hour_intervals.csv");
        }
    }
}