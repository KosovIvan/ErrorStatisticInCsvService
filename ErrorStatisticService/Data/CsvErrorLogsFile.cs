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
    }
}