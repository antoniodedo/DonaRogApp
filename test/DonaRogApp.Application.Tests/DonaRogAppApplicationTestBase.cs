using Volo.Abp.Modularity;

namespace DonaRogApp;

public abstract class DonaRogAppApplicationTestBase<TStartupModule> : DonaRogAppTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
