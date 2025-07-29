using DonaRogApp.Samples;
using Xunit;

namespace DonaRogApp.EntityFrameworkCore.Domains;

[Collection(DonaRogAppTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<DonaRogAppEntityFrameworkCoreTestModule>
{

}
