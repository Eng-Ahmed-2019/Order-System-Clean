namespace OrderSystem.Application.Exceptions
{
    public class ConfigurationException : Exception
    {
        public ConfigurationException(string m) : base(m) { }
    }
}