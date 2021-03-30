using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ShippingQuotes.Business.Integrations.Vendor1;
using ShippingQuotes.Business.Integrations.Vendor2;
using ShippingQuotes.Business.Integrations.Vendor3;
using ShippingQuotes.Business.Interfaces;
using ShippingQuotes.Business.Services;

namespace ShippingQuotes.Api
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddInjectedServices(this IServiceCollection services, IConfiguration config)
        {
            return services
                .AddSingleton(config)
                .AddScoped<IVendorUrlProvider, SettingsProvider>()
                .AddScoped<IVendorCredentialsProvider, SettingsProvider>()
                .AddScoped<IApiService, ApiService>()
                .AddScoped<IVendorService, Vendor1Service>()
                .AddScoped<IVendorService, Vendor2Service>()
                .AddScoped<IVendorService, Vendor3Service>()
                .AddScoped<IQuoteService, QuoteService>();
        }
    }
}