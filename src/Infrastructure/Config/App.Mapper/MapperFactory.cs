using System;
using System.Collections.Generic;
using System.Text;
using MicBeach.Util.ObjectMap;

namespace App.Mapper
{
    public static class MapperFactory
    {
        static IObjectMap objectMapper;
        static MapperFactory()
        {
            //初始化
            var autoMapper = new AutoMapMapper();
            autoMapper.Register();
            objectMapper = autoMapper;
        }

        #region 属性

        /// <summary>
        /// 对象映射转换器
        /// </summary>
        public static IObjectMap ObjectMapper
        {
            get
            {
                return objectMapper;
            }
        }

        #endregion
    }
}
