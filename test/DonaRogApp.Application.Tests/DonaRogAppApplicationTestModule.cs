using Volo.Abp.Modularity;

namespace DonaRogApp;

[DependsOn(
    typeof(DonaRogAppApplicationModule),
    typeof(DonaRogAppDomainTestModule)
)]
public class DonaRogAppApplicationTestModule : AbpModule
{

}
