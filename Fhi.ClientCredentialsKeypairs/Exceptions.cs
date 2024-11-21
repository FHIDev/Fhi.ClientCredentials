namespace Fhi.ClientCredentialsKeypairs;

[Serializable]
public class InvalidApiNameException : Exception
{
    public InvalidApiNameException() { }
    public InvalidApiNameException(string message) : base(message) { }
    public InvalidApiNameException(string message, Exception inner) : base(message, inner) { }
}

[Serializable]
public class ConfigurationException : Exception
{
    public ConfigurationException() { }
    public ConfigurationException(string message) : base(message) { }
    public ConfigurationException(string message, Exception inner) : base(message, inner) { }
}