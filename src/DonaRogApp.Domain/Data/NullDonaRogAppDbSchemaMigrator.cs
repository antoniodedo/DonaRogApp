using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace DonaRogApp.Data;

/* This is used if database provider does't define
 * IDonaRogAppDbSchemaMigrator implementation.
 */
public class NullDonaRogAppDbSchemaMigrator : IDonaRogAppDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
