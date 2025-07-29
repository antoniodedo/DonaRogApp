using DonaRogApp.Samples;
using Xunit;

namespace DonaRogApp.EntityFrameworkCore.Applications;

[Collection(DonaRogAppTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<DonaRogAppEntityFrameworkCoreTestModule>
{

}
