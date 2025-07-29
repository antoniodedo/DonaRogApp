using Xunit;

namespace DonaRogApp.EntityFrameworkCore;

[CollectionDefinition(DonaRogAppTestConsts.CollectionDefinitionName)]
public class DonaRogAppEntityFrameworkCoreCollection : ICollectionFixture<DonaRogAppEntityFrameworkCoreFixture>
{

}
