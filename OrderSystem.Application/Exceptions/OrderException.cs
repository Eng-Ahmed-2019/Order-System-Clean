namespace OrderSystem.Application.Exceptions
{
    public class OrderException : Exception
    {
        public OrderException(string m) : base(m) { }
    }
}