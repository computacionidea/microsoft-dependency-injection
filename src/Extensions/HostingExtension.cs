using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Unity.Microsoft.DependencyInjection
{
    public static class HostingExtension
    {
        private static ServiceProviderFactory _factory;


        public static IHostBuilder UseUnityServiceProvider(this IHostBuilder hostBuilder, IUnityContainer container = null, ServiceProviderMode mode = ServiceProviderMode.ChildContainerPerProvider)
        {
            _factory = new ServiceProviderFactory(container, mode);

            return hostBuilder.UseServiceProviderFactory<IUnityContainer>(_factory)
                              .ConfigureServices((context, services) =>
                              {
                                  services.Replace(ServiceDescriptor.Singleton<IServiceProviderFactory<IUnityContainer>>(_factory));
                                  services.Replace(ServiceDescriptor.Singleton<IServiceProviderFactory<IServiceCollection>>(_factory));
                              });
        }

        public static IWebHostBuilder UseUnityServiceProvider(this IWebHostBuilder hostBuilder, IUnityContainer container = null, ServiceProviderMode mode = ServiceProviderMode.ChildContainerPerProvider)
        {
            _factory = new ServiceProviderFactory(container, mode);

#if NETCOREAPP1_1
            return hostBuilder.ConfigureServices((services) =>
            {
                services.Replace(ServiceDescriptor.Singleton<IServiceProviderFactory<IUnityContainer>>(_factory));
                services.Replace(ServiceDescriptor.Singleton<IServiceProviderFactory<IServiceCollection>>(_factory));
            });
#else
            return hostBuilder.ConfigureServices((context, services) =>
            {
                services.Replace(ServiceDescriptor.Singleton<IServiceProviderFactory<IUnityContainer>>(_factory));
                services.Replace(ServiceDescriptor.Singleton<IServiceProviderFactory<IServiceCollection>>(_factory));
            });
#endif
        }
    }
}
