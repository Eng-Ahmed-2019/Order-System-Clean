namespace OrderSystem.Application.Exceptions
{
    public class UnauthorizedException : Exception
    {
        public UnauthorizedException(string m = "Invalid email or password") : base(m) { }
    }
}