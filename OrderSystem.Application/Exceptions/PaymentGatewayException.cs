namespace OrderSystem.Application.Exceptions
{
    public class PaymentGatewayException : Exception
    {
        public PaymentGatewayException(string m) : base(m) { }
    }
}