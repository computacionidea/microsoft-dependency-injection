using System;
using Microsoft.Extensions.DependencyInjection;
using Unity.Lifetime;

namespace Unity.Microsoft.DependencyInjection
{
    public class ServiceProviderFactory : IServiceProviderFactory<IUnityContainer>,
                                          IServiceProviderFactory<IServiceCollection>
    {
        #region Fields

        private readonly IUnityContainer _container;
        private readonly ServiceProviderMode _mode;
        private bool _firstChildSkipped;

        #endregion


        #region Constructors

        public ServiceProviderFactory(IUnityContainer container, ServiceProviderMode mode = ServiceProviderMode.ChildContainerPerProvider)
        {
            _container = container ?? new UnityContainer();
            _mode = mode;
            ((UnityContainer)_container).AddExtension(new MdiExtension());

            _container.RegisterInstance<IServiceProviderFactory<IUnityContainer>>(this, new ContainerControlledLifetimeManager());
            _container.RegisterInstance<IServiceProviderFactory<IServiceCollection>>(this, new ExternallyControlledLifetimeManager());
        }

        #endregion


        #region IServiceProviderFactory<IUnityContainer>

        public IServiceProvider CreateServiceProvider(IUnityContainer container)
        {
            return new ServiceProvider(container);
        }

        IUnityContainer IServiceProviderFactory<IUnityContainer>.CreateBuilder(IServiceCollection services)
        {
            return CreateServiceProviderContainer(services);
        }

        #endregion


        #region IServiceProviderFactory<IServiceCollection>

        public IServiceProvider CreateServiceProvider(IServiceCollection containerBuilder)
        {
            return new ServiceProvider(CreateServiceProviderContainer(containerBuilder));
        }

        IServiceCollection IServiceProviderFactory<IServiceCollection>.CreateBuilder(IServiceCollection services)
        {
            return services;
        }

        #endregion


        #region Implementation

        private IUnityContainer CreateServiceProviderContainer(IServiceCollection services)
        {
            IUnityContainer container;

            switch (_mode)
            {
                default:
                case ServiceProviderMode.ChildContainerPerProvider:
                    container = _container.CreateChildContainer();
                    new ServiceProviderFactory(container);
                    break;
                case ServiceProviderMode.ChildContainerPerProviderExceptFirstTime:
                    if (_firstChildSkipped)
                    {
                        goto case ServiceProviderMode.ChildContainerPerProvider;
                    }
                    else
                    {
                        _firstChildSkipped = true;
                        goto case ServiceProviderMode.SameContainerAlways;
                    }
                case ServiceProviderMode.SameContainerAlways:
                    container = _container;
                    break;
            }

            return ((UnityContainer)container).AddExtension(new MdiExtension())
                                              .AddServices(services);
        }

        #endregion
    }
}
