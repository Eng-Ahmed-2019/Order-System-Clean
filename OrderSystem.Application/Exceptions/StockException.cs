namespace OrderSystem.Application.Exceptions
{
    public class StockException : Exception
    {
        public StockException(string m) : base(m) { }
    }
}