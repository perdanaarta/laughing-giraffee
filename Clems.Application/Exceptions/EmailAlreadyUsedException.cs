namespace Clems.Application.Exceptions;

public class EmailAlreadyUsedException(string email) : Exception($"Email {email} already used");