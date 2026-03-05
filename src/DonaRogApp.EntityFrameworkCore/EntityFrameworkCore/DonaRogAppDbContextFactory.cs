using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace DonaRogApp.EntityFrameworkCore;

/* This class is needed for EF Core console commands
 * (like Add-Migration and Update-Database commands) */
public class DonaRogAppDbContextFactory : IDesignTimeDbContextFactory<DonaRogAppDbContext>
{
    public DonaRogAppDbContext CreateDbContext(string[] args)
    {
        var configuration = BuildConfiguration();
        
        DonaRogAppEfCoreEntityExtensionMappings.Configure();

        var builder = new DbContextOptionsBuilder<DonaRogAppDbContext>()
            .UseNpgsql(configuration.GetConnectionString("Default"));
        
        return new DonaRogAppDbContext(builder.Options);
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../DonaRogApp.DbMigrator/"))
            .AddJsonFile("appsettings.json", optional: false);

        return builder.Build();
    }
}
