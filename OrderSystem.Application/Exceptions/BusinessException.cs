namespace OrderSystem.Application.Exceptions
{
    public class BusinessException : Exception
    {
        public BusinessException(string m) : base(m) { }
    }
}