using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Common.VNextFramework.Extensions
{
    public static class AutoMapperExtensions
    {
        private static IServiceProvider _serviceProvider;

        public static void UseStateAutoMapper(this IApplicationBuilder applicationBuilder)
        {
            _serviceProvider = applicationBuilder.ApplicationServices;
        }

        public static TDestination MapTo<TSource, TDestination>(this TSource source)
        {
            var mapper = _serviceProvider.GetRequiredService<IMapper>();

            return mapper.Map<TSource, TDestination>(source);
        }

        public static TDestination MapTo<TDestination>(this object source)
        {
            var mapper = _serviceProvider.GetRequiredService<IMapper>();

            return mapper.Map<TDestination>(source);
        }
    }
}
