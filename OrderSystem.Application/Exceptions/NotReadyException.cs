namespace OrderSystem.Application.Exceptions
{
    public class NotReadyException : Exception
    {
        public NotReadyException(string m) : base(m) { }
    }
}