namespace OrderSystem.Application.Exceptions
{
    public class ValidationException : Exception
    {
        public Dictionary<string, string[]> Errors { get; } = new Dictionary<string, string[]>();

        public ValidationException(Dictionary<string, string[]> errors)
        {
            Errors = errors;
        }
    }
}