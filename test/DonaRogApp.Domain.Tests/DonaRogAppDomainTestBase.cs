using Volo.Abp.Modularity;

namespace DonaRogApp;

/* Inherit from this class for your domain layer tests. */
public abstract class DonaRogAppDomainTestBase<TStartupModule> : DonaRogAppTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
