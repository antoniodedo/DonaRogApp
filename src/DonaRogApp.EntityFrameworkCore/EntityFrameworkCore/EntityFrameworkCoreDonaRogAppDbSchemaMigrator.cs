using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using DonaRogApp.Data;
using Volo.Abp.DependencyInjection;

namespace DonaRogApp.EntityFrameworkCore;

public class EntityFrameworkCoreDonaRogAppDbSchemaMigrator
    : IDonaRogAppDbSchemaMigrator, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public EntityFrameworkCoreDonaRogAppDbSchemaMigrator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolving the DonaRogAppDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<DonaRogAppDbContext>()
            .Database
            .MigrateAsync();
    }
}
