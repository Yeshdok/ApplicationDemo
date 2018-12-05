using MicBeach.Mvc.CustomModelDisplayName;
using MicBeach.Mvc.DataAnnotationsModelValidatorConfig;
using MicBeach.Web.DI;
using System;
using System.Collections.Generic;
using System.Text;

namespace Site.Cms.Config
{
    public static class MvcConfig
    {
        public static void Init()
        {
            MvcDataValidation.AddCustomDataAnnotationsModelValidatorProvider(ServiceProviderConfig.ServiceProvider);//添加自定义数据验证
            MvcCustomModelDisplayProvider.Register(ServiceProviderConfig.ServiceProvider);
        }
    }
}