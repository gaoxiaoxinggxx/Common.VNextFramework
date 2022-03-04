using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;

namespace Common.VNextFramework.Extensions
{
    public static class InjectionExtensions
    {
        public static void AddTransientScan(this IServiceCollection services, Type serviceType)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (serviceType == null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }
            AddServices(services, serviceType, ServiceLifetime.Transient);
        }
        public static void AddTransientScan(this IServiceCollection services, string assembly)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (assembly == null)
            {
                throw new ArgumentNullException(assembly);
            }
            AddServices(services, assembly, ServiceLifetime.Transient);
        }
        public static void AddTransientScan<TService>(this IServiceCollection services) where TService : class
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (typeof(TService) == null)
            {
                throw new ArgumentNullException(nameof(TService));
            }
            AddServices(services, typeof(TService), ServiceLifetime.Transient);
        }
        public static void AddScopedScan(this IServiceCollection services, Type serviceType)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (serviceType == null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }
            AddServices(services, serviceType, ServiceLifetime.Scoped);
        }
        public static void AddScopedScan(this IServiceCollection services, string assembly)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (assembly == null)
            {
                throw new ArgumentNullException(assembly);
            }
            AddServices(services, assembly, ServiceLifetime.Scoped);
        }
        public static void AddScopedScan<TService>(this IServiceCollection services) where TService : class
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (typeof(TService) == null)
            {
                throw new ArgumentNullException(nameof(TService));
            }
            AddServices(services, typeof(TService), ServiceLifetime.Scoped);
        }
        public static void AddSingletonScan(this IServiceCollection services, Type serviceType)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (serviceType == null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }
            AddServices(services, serviceType, ServiceLifetime.Singleton);
        }
        public static void AddSingletonScan(this IServiceCollection services, string assembly)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (assembly == null)
            {
                throw new ArgumentNullException(assembly);
            }
            AddServices(services, assembly, ServiceLifetime.Singleton);
        }
        public static void AddSingletonScan<TService>(this IServiceCollection services) where TService : class
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (typeof(TService) == null)
            {
                throw new ArgumentNullException(nameof(TService));
            }
            AddServices(services, typeof(TService), ServiceLifetime.Singleton);
        }

        private static void AddServices(IServiceCollection services, Type type, ServiceLifetime lifetime)
        {
            var types = AssemblyScan(type.Assembly);
            AddServices(services, types, lifetime);
        }
        private static void AddServices(IServiceCollection services, string assembly, ServiceLifetime lifetime)
        {
            var types = AssemblyScan(Assembly.Load(assembly));
            AddServices(services, types, lifetime);
        }
        private static void AddServices(IServiceCollection services, Type[] types, ServiceLifetime lifetime)
        {
            foreach (var type in types)
            {
                Add(services, type, type, lifetime);

                if (type.BaseType != null && !type.BaseType.IsInterface)
                {
                    Add(services, type.BaseType, type, lifetime);
                }

                var interfaces = type.GetInterfaces();
                foreach (var _interface in interfaces)
                {
                    Add(services, _interface, type, lifetime);
                }
            }
        }

        private static IServiceCollection Add(IServiceCollection collection, Type serviceType, Type implementationType, ServiceLifetime lifetime)
        {
            var descriptor = new ServiceDescriptor(serviceType, implementationType, lifetime);
            collection.Add(descriptor);
            return collection;
        }

        public static Type[] AssemblyScan(Assembly assembly)
        {
            var types = assembly.GetTypes();
            var injectionTypes = types.Where(c => c.IsClass && c.IsPublic && !c.IsSealed && !c.IsGenericType && !c.IsAbstract && !c.IsSubclassOf(typeof(Attribute))).ToArray();
            return injectionTypes;
        }
    }
}
