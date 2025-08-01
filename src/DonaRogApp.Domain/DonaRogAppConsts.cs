using Volo.Abp.Identity;

namespace DonaRogApp;

public static class DonaRogAppConsts
{
    public const string DbTablePrefix = "App_";
    public const string? DbSchema = null;
    public const string AdminEmailDefaultValue = IdentityDataSeedContributor.AdminEmailDefaultValue;
    public const string AdminPasswordDefaultValue = IdentityDataSeedContributor.AdminPasswordDefaultValue;
}
