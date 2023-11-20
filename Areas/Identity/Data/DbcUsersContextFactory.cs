using dbc_Dave.Data;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;

public class DbcUsersContextFactory : IDesignTimeDbContextFactory<dbc_UsersContext>
{
    public dbc_UsersContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<dbc_UsersContext>();
        optionsBuilder.UseSqlServer("YourConnectionString");
        return new dbc_UsersContext(optionsBuilder.Options);
    }
}