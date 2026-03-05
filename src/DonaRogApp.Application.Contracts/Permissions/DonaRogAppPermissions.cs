namespace DonaRogApp.Permissions;

public static class DonaRogAppPermissions
{
    public const string GroupName = "DonaRogApp";

    public static class Donors
    {
        public const string Default = GroupName + ".Donors";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }

    public static class SegmentationRules
    {
        public const string Default = GroupName + ".SegmentationRules";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
        public const string ExecuteBatch = Default + ".ExecuteBatch";
    }
    
    //Add your own permission names. Example:
    //public const string MyPermission1 = GroupName + ".MyPermission1";
}
