using DonaRogApp.Localization;
using Volo.Abp.Application.Services;

namespace DonaRogApp;

/* Inherit your application services from this class.
 */
public abstract class DonaRogAppAppService : ApplicationService
{
    protected DonaRogAppAppService()
    {
        LocalizationResource = typeof(DonaRogAppResource);
    }
}
