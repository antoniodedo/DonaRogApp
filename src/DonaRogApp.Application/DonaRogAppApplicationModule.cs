using DinkToPdf;
using DinkToPdf.Contracts;
using DonaRogApp.Application.Segmentation;
using DonaRogApp.Domain.Storage;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using Volo.Abp.Account;
using Volo.Abp.Identity;
using Volo.Abp.AutoMapper;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Modularity;
using Volo.Abp.TenantManagement;

namespace DonaRogApp;

[DependsOn(
    typeof(DonaRogAppDomainModule),
    typeof(DonaRogAppApplicationContractsModule),
    typeof(AbpPermissionManagementApplicationModule),
    typeof(AbpFeatureManagementApplicationModule),
    typeof(AbpIdentityApplicationModule),
    typeof(AbpAccountApplicationModule),
    typeof(AbpTenantManagementApplicationModule),
    typeof(AbpSettingManagementApplicationModule)
    )]
public class DonaRogAppApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();
        
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<DonaRogAppApplicationModule>();
        });
        
        // Configure File Storage
        Configure<FileStorageOptions>(configuration.GetSection("FileStorage"));
        
        // Configure DinkToPdf for HTML to PDF conversion
        ConfigurePdfGeneration(context);
    }
    
    public override async Task OnApplicationInitializationAsync(ApplicationInitializationContext context)
    {
        await context.AddBackgroundWorkerAsync<SegmentationBackgroundWorker>();
    }
    
    private void ConfigurePdfGeneration(ServiceConfigurationContext context)
    {
        // Register DinkToPdf converter (wkhtmltopdf wrapper)
        // Note: wkhtmltopdf native library must be installed on the server
        // For Windows: download from https://wkhtmltopdf.org/downloads.html
        // For Linux: apt-get install wkhtmltopdf
        // Use factory to allow test mocking
        context.Services.AddSingleton<IConverter>(sp => new SynchronizedConverter(new PdfTools()));
    }
}
