namespace KMSTraining.API.Domain.Exceptions;

/// <summary>
/// Exception thrown when a domain validation fails
/// </summary>
public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
    public DomainException(string message, Exception innerException) : base(message, innerException) { }
}

/// <summary>
/// Exception thrown when a requested entity is not found
/// </summary>
public class EntityNotFoundException : DomainException
{
    public EntityNotFoundException(string entityName, int id)
        : base($"{entityName} with ID {id} was not found.") { }

    public EntityNotFoundException(string message)
        : base(message) { }
}

/// <summary>
/// Exception thrown when an entity already exists
/// </summary>
public class DuplicateEntityException : DomainException
{
    public DuplicateEntityException(string message)
        : base(message) { }
}

/// <summary>
/// Exception thrown when authentication fails
/// </summary>
public class AuthenticationException : DomainException
{
    public AuthenticationException(string message)
        : base(message) { }
}

/// <summary>
/// Exception thrown when authorization fails
/// </summary>
public class AuthorizationException : DomainException
{
    public AuthorizationException(string message)
        : base(message) { }
}
