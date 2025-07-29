using Volo.Abp.Modularity;

namespace DonaRogApp;

[DependsOn(
    typeof(DonaRogAppDomainModule),
    typeof(DonaRogAppTestBaseModule)
)]
public class DonaRogAppDomainTestModule : AbpModule
{

}
