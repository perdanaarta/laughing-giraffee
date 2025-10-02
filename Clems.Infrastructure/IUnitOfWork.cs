namespace Clems.Infrastructure;

public interface IUnitOfWork
{
    Task SaveAsync();
}