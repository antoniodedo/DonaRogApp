using DonaRogApp.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace DonaRogApp.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class DonaRogAppController : AbpControllerBase
{
    protected DonaRogAppController()
    {
        LocalizationResource = typeof(DonaRogAppResource);
    }
}
