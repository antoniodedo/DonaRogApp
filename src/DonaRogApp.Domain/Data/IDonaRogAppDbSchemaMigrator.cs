using System.Threading.Tasks;

namespace DonaRogApp.Data;

public interface IDonaRogAppDbSchemaMigrator
{
    Task MigrateAsync();
}
