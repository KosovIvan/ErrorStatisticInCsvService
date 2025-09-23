using ErrorStatisticService.Models;

namespace ErrorStatisticService.Services
{
    public class RandomErrorLogGeneratorService : IRandomGeneratorService<ErrorLog>
    {
        private const int ProductsLength = 5;
        private const int VersionsLength = 20;
        private const int ErrorCodesLength = 100;
        private DateTime _timestamp = DateTime.Today;
        private readonly string[] _products;
        private readonly string[] _versions;
        private readonly string[] _errorCodes;
        private readonly Random rnd = new Random();

        private void InitStringArray(out string[] arr, int length, string name, Func<int, string> format)
        {
            arr = new string[length];
            for (int i = 0; i < length; i++)
            {
                arr[i] = name + format(i + 1);
            }
        }
        public RandomErrorLogGeneratorService() {
            InitStringArray(out _products, ProductsLength, "Product", s => s.ToString());
            InitStringArray(out _versions, VersionsLength, "v", s => $"{s}.0");
            InitStringArray(out _errorCodes, ErrorCodesLength, "ER", s => s.ToString("D3"));
        }
        public ErrorLog Generate()
        {
            var timestamp = _timestamp.AddHours(rnd.Next(24)).AddMinutes(rnd.Next(60)).AddSeconds(rnd.Next(60));
            Severity severity = (Severity)rnd.Next(4);
            string product = _products[rnd.Next(ProductsLength)];
            string version = _versions[rnd.Next(VersionsLength)];
            string errorCode = _errorCodes[rnd.Next(ErrorCodesLength)];

            return new ErrorLog { 
                Timestamp = timestamp,  
                Severity = severity, 
                Product = product, 
                Version = version, 
                ErrorCode = errorCode 
            };
        }
    }
}