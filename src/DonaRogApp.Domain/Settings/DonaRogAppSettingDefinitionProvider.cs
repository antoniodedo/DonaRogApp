using Volo.Abp.Settings;

namespace DonaRogApp.Settings;

public class DonaRogAppSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(DonaRogAppSettings.MySetting1));
    }
}
