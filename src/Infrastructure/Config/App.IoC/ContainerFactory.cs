using MicBeach.Util.Drawing;
using MicBeach.Util.IoC;
using MicBeach.Util.Serialize;
using MicBeach.Web.Utility;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using MicBeach.Web.DI;
using MicBeach.Serialize.Json.JsonNet;

namespace App.IoC
{
    public static class ContainerFactory
    {
        public static IServiceProvider GetServiceProvider(IServiceCollection services)
        {
            return ServiceProviderConfig.BuildServiceProvider();
        }

        public static void RegisterServices(IServiceCollection services)
        {
            ServiceProviderConfig.RegisterServiceMethod = RegisterTypes;
            ServiceProviderConfig.RegisterServices(services);
            ContainerManager.DefaultServiceCollection = services;
        }

        static void RegisterTypes()
        {
            var container = new ServiceProviderContainer();
            container.DefaultRegister();
            container.RegisterType(typeof(IJsonSerializer), typeof(JsonNetSerializer));
            ContainerManager.Container = container;
        }
    }
}
