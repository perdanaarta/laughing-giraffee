namespace Clems.Application.Exceptions;

public class UserNotFoundException : Exception
{
    public UserNotFoundException(string email) : base($"User with email {email} not found")
    {
    }

    public UserNotFoundException(Guid id) : base($"User with id {id} not found")
    {
    }
}