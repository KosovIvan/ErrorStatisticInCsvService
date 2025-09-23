namespace ErrorStatisticService.Models
{
    public class ErrorLog
    {
        public DateTime Timestamp { get; set; }
        public Severity Severity { get; set; }
        public string Product {  get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public string ErrorCode { get; set; } = string.Empty;
    }

    public enum Severity
    {
        Critical,
        High,
        Normal,
        Low
    }
}