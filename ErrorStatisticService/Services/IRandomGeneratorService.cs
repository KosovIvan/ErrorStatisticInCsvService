namespace ErrorStatisticService.Services
{
    public interface IRandomGeneratorService<T>
    {
        public T Generate();
    }
}