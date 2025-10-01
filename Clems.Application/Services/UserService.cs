using Clems.Domain.Event;
using Clems.Domain.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharedKernel.Abstraction;

namespace Clems.Application.Services;

public class UserService(
    IUserStore<User> store,
    IOptions<IdentityOptions> optionsAccessor,
    IPasswordHasher<User> passwordHasher,
    IEnumerable<IUserValidator<User>> userValidators,
    IEnumerable<IPasswordValidator<User>> passwordValidators,
    ILookupNormalizer keyNormalizer,
    IdentityErrorDescriber errors,
    IServiceProvider services,
    ILogger<UserManager<User>> logger,
    IMediator mediator)
    : UserManager<User>(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer,
        errors, services, logger)
{
    public async Task<IdentityResult> CreateAsync(string email, string password)
    {
        var user = new User { UserName = email, Email = email };

        var res = await base.CreateAsync(user, password);

        if (res.Succeeded)
        {
            await mediator.PublishAsync(new UserCreatedEvent(user.Id));
        }

        return res;
    }
}