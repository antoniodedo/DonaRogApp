using DonaRogApp.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace DonaRogApp.Permissions;

public class DonaRogAppPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(DonaRogAppPermissions.GroupName);

        // Donors
        var donors = myGroup.AddPermission(DonaRogAppPermissions.Donors.Default, L("Permission:Donors"));
        donors.AddChild(DonaRogAppPermissions.Donors.Create, L("Permission:Create"));
        donors.AddChild(DonaRogAppPermissions.Donors.Edit, L("Permission:Edit"));
        donors.AddChild(DonaRogAppPermissions.Donors.Delete, L("Permission:Delete"));

        // Segmentation Rules
        var segmentationRules = myGroup.AddPermission(DonaRogAppPermissions.SegmentationRules.Default, L("Permission:SegmentationRules"));
        segmentationRules.AddChild(DonaRogAppPermissions.SegmentationRules.Create, L("Permission:Create"));
        segmentationRules.AddChild(DonaRogAppPermissions.SegmentationRules.Edit, L("Permission:Edit"));
        segmentationRules.AddChild(DonaRogAppPermissions.SegmentationRules.Delete, L("Permission:Delete"));
        segmentationRules.AddChild(DonaRogAppPermissions.SegmentationRules.ExecuteBatch, L("Permission:ExecuteBatch"));

        //Define your own permissions here. Example:
        //myGroup.AddPermission(DonaRogAppPermissions.MyPermission1, L("Permission:MyPermission1"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<DonaRogAppResource>(name);
    }
}
