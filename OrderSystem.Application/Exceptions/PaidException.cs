namespace OrderSystem.Application.Exceptions
{
    public class PaidException : Exception
    {
        public PaidException(string m) : base(m) { }
    }
}