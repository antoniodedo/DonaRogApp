using Microsoft.Extensions.Localization;
using DonaRogApp.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace DonaRogApp;

[Dependency(ReplaceServices = true)]
public class DonaRogAppBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<DonaRogAppResource> _localizer;

    public DonaRogAppBrandingProvider(IStringLocalizer<DonaRogAppResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
