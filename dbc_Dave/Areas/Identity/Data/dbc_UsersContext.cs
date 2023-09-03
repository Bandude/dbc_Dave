using dbc_Dave.Areas.Identity.Data;
using dbc_Dave.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace dbc_Dave.Data;

public class dbc_UsersContext : IdentityDbContext<User>
{
    public dbc_UsersContext(DbContextOptions<dbc_UsersContext> options)
        : base(options)
    {
    }
    public DbSet<DataQuery> Queries { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {

        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }
}
