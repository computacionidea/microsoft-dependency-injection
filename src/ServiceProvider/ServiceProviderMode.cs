using System;
using System.Collections.Generic;
using System.Text;

namespace Unity.Microsoft.DependencyInjection
{
    /// <summary>
    /// Determines how ServiceProviderFactory will use the container when a ServiceProvider is created.
    /// 
    /// </summary>
    public enum ServiceProviderMode
    {
        /// <summary>
        /// Creates a child container for each ServiceProvider requested.
        /// </summary>
        ChildContainerPerProvider,
        /// <summary>
        /// The first ServiceProvider will have the container. Subsequent calls will behave same as ChildContainerPerProvider
        /// In ChildContainerPerProvider mode, IHostBuilder. always create a child container and register
        /// every component in it. Leaving the root container with practically empty. This cause problems
        /// when using SingletonLifetimeManager. Any component with singleton lifetime will not have access to
        /// any other dependency.
        /// This mode fix this. When IHostBuilder creates the first ServiceProvider, this will be backed by
        /// the original container, not a child.
        /// </summary>
        ChildContainerPerProviderExceptFirstTime,
        /// <summary>
        /// Use the same container for every ServiceProvider requested.
        /// </summary>
        SameContainerAlways
    }
}
