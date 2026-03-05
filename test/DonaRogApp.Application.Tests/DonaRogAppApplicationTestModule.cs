using System;
using DinkToPdf;
using DinkToPdf.Contracts;
using DinkToPdf.EventDefinitions;
using DonaRogApp.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Sqlite;
using Volo.Abp.Modularity;

namespace DonaRogApp;

[DependsOn(
    typeof(DonaRogAppApplicationModule),
    typeof(DonaRogAppEntityFrameworkCoreModule),
    typeof(DonaRogAppDomainTestModule),
    typeof(AbpEntityFrameworkCoreSqliteModule)
)]
public class DonaRogAppApplicationTestModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var sqliteConnection = CreateDatabaseAndGetConnection();

        Configure<AbpDbContextOptions>(options =>
        {
            options.Configure(abpDbContextConfigurationContext =>
            {
                abpDbContextConfigurationContext.DbContextOptions.UseSqlite(sqliteConnection);
            });
        });

        // Replace DinkToPdf with a mock to avoid native DLL issues in tests
        context.Services.AddSingleton<IConverter>(new MockPdfConverter());
    }

    private static SqliteConnection CreateDatabaseAndGetConnection()
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();

        new DonaRogAppDbContext(
            new DbContextOptionsBuilder<DonaRogAppDbContext>().UseSqlite(connection).Options
        ).GetService<IRelationalDatabaseCreator>().CreateTables();

        return connection;
    }

    private class MockPdfConverter : IConverter
    {
        public event EventHandler<PhaseChangedArgs>? PhaseChanged;
        public event EventHandler<ProgressChangedArgs>? ProgressChanged;
        public event EventHandler<FinishedArgs>? Finished;
        public event EventHandler<ErrorArgs>? Error;
        public event EventHandler<WarningArgs>? Warning;

        public byte[] Convert(IDocument document)
        {
            return new byte[] { 0x25, 0x50, 0x44, 0x46 }; // "%PDF" header
        }
    }
}
