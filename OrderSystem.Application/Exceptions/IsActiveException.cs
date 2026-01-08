namespace OrderSystem.Application.Exceptions
{
    public class IsActiveException : Exception
    {
        public IsActiveException(string m) : base(m) { }
    }
}