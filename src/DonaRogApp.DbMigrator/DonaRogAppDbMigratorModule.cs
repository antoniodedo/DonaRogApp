using DonaRogApp.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace DonaRogApp.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(DonaRogAppEntityFrameworkCoreModule),
    typeof(DonaRogAppApplicationContractsModule)
)]
public class DonaRogAppDbMigratorModule : AbpModule
{
}
