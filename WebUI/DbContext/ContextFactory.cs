using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace WebUI.DbContext
{
    public class ContextFactory : IDesignTimeDbContextFactory<AccountDbContext>
    {
        public AccountDbContext CreateDbContext(string[] args)
        {
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile("appsettings.json");
            var config = builder.Build();
            string connectionString = config.GetConnectionString("DefaultConnection");
            var optionsBuilder = new DbContextOptionsBuilder<AccountDbContext>();
            DbContextOptions<AccountDbContext> options = optionsBuilder
                .UseSqlServer(connectionString).Options;
            return new AccountDbContext(optionsBuilder.Options);
        }
    }
}
