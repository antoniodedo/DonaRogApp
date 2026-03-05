using System;
using System.Linq;
using DinkToPdf;
using DinkToPdf.Contracts;
using DinkToPdf.EventDefinitions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Sqlite;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.Uow;

namespace DonaRogApp.EntityFrameworkCore;

[DependsOn(
    typeof(DonaRogAppApplicationTestModule),
    typeof(DonaRogAppEntityFrameworkCoreModule),
    typeof(AbpEntityFrameworkCoreSqliteModule)
)]
public class DonaRogAppEntityFrameworkCoreTestModule : AbpModule
{
    private SqliteConnection? _sqliteConnection;

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<FeatureManagementOptions>(options =>
        {
            options.SaveStaticFeaturesToDatabase = false;
            options.IsDynamicFeatureStoreEnabled = false;
        });
        Configure<PermissionManagementOptions>(options =>
        {
            options.SaveStaticPermissionsToDatabase = false;
            options.IsDynamicPermissionStoreEnabled = false;
        });
        context.Services.AddAlwaysDisableUnitOfWorkTransaction();

        ConfigureInMemorySqlite(context.Services);
    }

    private void ConfigureInMemorySqlite(IServiceCollection services)
    {
        _sqliteConnection = CreateDatabaseAndGetConnection();

        services.Configure<AbpDbContextOptions>(options =>
        {
            options.Configure(context =>
            {
                context.DbContextOptions.UseSqlite(_sqliteConnection);
            });
        });

        // Remove and replace DinkToPdf registration to avoid native DLL dependency
        var converterDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IConverter));
        if (converterDescriptor != null)
        {
            services.Remove(converterDescriptor);
        }
        services.AddSingleton<IConverter>(new MockPdfConverter());
    }

    public override void OnApplicationShutdown(ApplicationShutdownContext context)
    {
        _sqliteConnection?.Dispose();
    }

    private static SqliteConnection CreateDatabaseAndGetConnection()
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<DonaRogAppDbContext>()
            .UseSqlite(connection)
            .Options;

        using (var context = new DonaRogAppDbContext(options))
        {
            context.GetService<IRelationalDatabaseCreator>().CreateTables();
        }

        return connection;
    }

    private class MockPdfConverter : IConverter
    {
#pragma warning disable CS0067
        public event EventHandler<PhaseChangedArgs>? PhaseChanged;
        public event EventHandler<ProgressChangedArgs>? ProgressChanged;
        public event EventHandler<FinishedArgs>? Finished;
        public event EventHandler<ErrorArgs>? Error;
        public event EventHandler<WarningArgs>? Warning;
#pragma warning restore CS0067

        public byte[] Convert(IDocument document)
        {
            return new byte[] { 0x25, 0x50, 0x44, 0x46 }; // "%PDF" header
        }
    }
}
