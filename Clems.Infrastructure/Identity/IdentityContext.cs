using Clems.Domain.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Clems.Infrastructure.Identity;

public class IdentityContext(DbContextOptions<IdentityContext> options) : IdentityDbContext<User, IdentityRole<Guid>, Guid>(options)
{
    
}