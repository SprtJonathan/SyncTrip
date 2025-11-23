namespace SyncTrip.Core.Exceptions;

/// <summary>
/// Exception levée lors d'une violation de règle métier dans le domaine.
/// </summary>
public class DomainException : Exception
{
    public DomainException(string message) : base(message)
    {
    }

    public DomainException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
