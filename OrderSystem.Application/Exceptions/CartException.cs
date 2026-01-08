namespace OrderSystem.Application.Exceptions
{
    public class CartException : Exception
    {
        public CartException(string m) : base(m) { }
    }
}