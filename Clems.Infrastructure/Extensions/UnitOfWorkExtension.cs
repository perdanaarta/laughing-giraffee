namespace Clems.Infrastructure.Extensions;

public static class UnitOfWorkExtension
{
    public static void AddUnitOfWork(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
    }
}